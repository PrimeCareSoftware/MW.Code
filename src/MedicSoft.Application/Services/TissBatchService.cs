using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing TISS batches
    /// </summary>
    public class TissBatchService : ITissBatchService
    {
        private readonly ITissBatchRepository _batchRepository;
        private readonly ITissGuideRepository _guideRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IHealthInsuranceOperatorRepository _operatorRepository;
        private readonly ITissXmlGeneratorService _xmlGenerator;
        private readonly IMapper _mapper;

        public TissBatchService(
            ITissBatchRepository batchRepository,
            ITissGuideRepository guideRepository,
            IClinicRepository clinicRepository,
            IHealthInsuranceOperatorRepository operatorRepository,
            ITissXmlGeneratorService xmlGenerator,
            IMapper mapper)
        {
            _batchRepository = batchRepository;
            _guideRepository = guideRepository;
            _clinicRepository = clinicRepository;
            _operatorRepository = operatorRepository;
            _xmlGenerator = xmlGenerator;
            _mapper = mapper;
        }

        public async Task<TissBatchDto> CreateAsync(CreateTissBatchDto dto, string tenantId)
        {
            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(dto.ClinicId, tenantId);
            if (clinic == null)
            {
                throw new InvalidOperationException($"Clinic with ID {dto.ClinicId} not found");
            }

            // Validate operator exists
            var operatorEntity = await _operatorRepository.GetByIdAsync(dto.OperatorId, tenantId);
            if (operatorEntity == null)
            {
                throw new InvalidOperationException($"Health insurance operator with ID {dto.OperatorId} not found");
            }

            // Execute batch creation in transaction to ensure data consistency
            TissBatch? batch = null;
            await _batchRepository.ExecuteInTransactionAsync(async () =>
            {
                // Generate batch number
                var batchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                batch = new TissBatch(
                    dto.ClinicId,
                    dto.OperatorId,
                    batchNumber,
                    tenantId
                );

                await _batchRepository.AddAsync(batch);
            });

            if (batch == null)
            {
                throw new InvalidOperationException("Failed to create TISS batch");
            }

            // Reload with navigation properties
            var result = await _batchRepository.GetWithGuidesAsync(batch.Id, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> AddGuideAsync(Guid batchId, Guid guideId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            // Execute guide addition in transaction to ensure data consistency
            await _batchRepository.ExecuteInTransactionAsync(async () =>
            {
                // Verify guide belongs to same operator
                var guideInsurance = guide.PatientHealthInsurance;
                // Note: Would need to navigate to operator through plan, assuming this is validated elsewhere

                batch.AddGuide(guide);
                
                await _batchRepository.UpdateAsync(batch);
            });

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> RemoveGuideAsync(Guid batchId, Guid guideId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            // Execute guide removal in transaction to ensure data consistency
            await _batchRepository.ExecuteInTransactionAsync(async () =>
            {
                batch.RemoveGuide(guideId);
                
                await _batchRepository.UpdateAsync(batch);
            });

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissXmlGenerationResultDto> GenerateXmlAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                return new TissXmlGenerationResultDto
                {
                    Success = false,
                    ErrorMessage = $"TISS batch with ID {batchId} not found"
                };
            }

            try
            {
                // Define output directory (could be configurable)
                var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "TissXml", tenantId);
                Directory.CreateDirectory(outputDir);

                // Generate XML
                var xmlFilePath = await _xmlGenerator.GenerateBatchXmlAsync(batch, outputDir);
                var xmlFileName = Path.GetFileName(xmlFilePath);

                // Execute update in transaction to ensure data consistency
                await _batchRepository.ExecuteInTransactionAsync(async () =>
                {
                    // Update batch with XML info
                    batch.GenerateXml(xmlFileName, xmlFilePath);
                    
                    await _batchRepository.UpdateAsync(batch);
                });

                return new TissXmlGenerationResultDto
                {
                    Success = true,
                    XmlFilePath = xmlFilePath,
                    XmlFileName = xmlFileName
                };
            }
            catch (Exception ex)
            {
                return new TissXmlGenerationResultDto
                {
                    Success = false,
                    ErrorMessage = $"Failed to generate XML: {ex.Message}"
                };
            }
        }

        public async Task<TissBatchDto> MarkAsReadyToSendAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            batch.MarkAsReadyToSend();
            
            await _batchRepository.UpdateAsync(batch);
            await _batchRepository.SaveChangesAsync();

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> SubmitAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            // Generate protocol number (simulated)
            var protocolNumber = $"PROT-{DateTime.UtcNow:yyyyMMddHHmmss}";

            batch.Submit(protocolNumber);
            
            await _batchRepository.UpdateAsync(batch);
            await _batchRepository.SaveChangesAsync();

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> ProcessResponseAsync(Guid batchId, ProcessBatchResponseDto dto, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            // Execute response processing in transaction to ensure data consistency
            await _batchRepository.ExecuteInTransactionAsync(async () =>
            {
                // Mark as processing first
                if (batch.Status == BatchStatus.Sent)
                {
                    batch.MarkAsProcessing();
                }

                // Process batch-level response
                batch.ProcessResponse(
                    dto.ResponseXmlFileName,
                    dto.ApprovedAmount,
                    dto.GlosedAmount
                );

                // Process guide-level responses
                foreach (var guideResponse in dto.GuideResponses)
                {
                    var guide = batch.Guides.FirstOrDefault(g => g.GuideNumber == guideResponse.GuideNumber);
                    if (guide != null)
                    {
                        if (guideResponse.ApprovedAmount.HasValue)
                        {
                            guide.Approve(guideResponse.ApprovedAmount.Value);
                        }
                        else if (!string.IsNullOrWhiteSpace(guideResponse.GlossReason))
                        {
                            guide.Reject(guideResponse.GlossReason);
                        }

                        // Process procedure-level responses
                        foreach (var procResponse in guideResponse.ProcedureResponses)
                        {
                            var procedure = guide.Procedures.FirstOrDefault(p => p.Id == procResponse.ProcedureId);
                            if (procedure != null)
                            {
                                procedure.ProcessOperatorResponse(
                                    null,
                                    procResponse.ApprovedAmount,
                                    procResponse.GlossReason
                                );
                            }
                        }
                        
                        await _guideRepository.UpdateAsync(guide);
                    }
                }

                await _batchRepository.UpdateAsync(batch);
            });

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> MarkAsPaidAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            // Execute payment marking in transaction to ensure data consistency
            await _batchRepository.ExecuteInTransactionAsync(async () =>
            {
                batch.MarkAsPaid();
                
                // Also mark all guides as paid
                foreach (var guide in batch.Guides)
                {
                    if (guide.Status == GuideStatus.Approved || guide.Status == GuideStatus.PartiallyApproved)
                    {
                        guide.MarkAsPaid();
                        await _guideRepository.UpdateAsync(guide);
                    }
                }

                await _batchRepository.UpdateAsync(batch);
            });

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<TissBatchDto> RejectAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {batchId} not found");
            }

            batch.Reject();
            
            await _batchRepository.UpdateAsync(batch);
            await _batchRepository.SaveChangesAsync();

            var result = await _batchRepository.GetWithGuidesAsync(batchId, tenantId);
            return _mapper.Map<TissBatchDto>(result);
        }

        public async Task<IEnumerable<TissBatchDto>> GetAllAsync(string tenantId)
        {
            var batches = await _batchRepository.GetAllAsync(tenantId);
            return _mapper.Map<IEnumerable<TissBatchDto>>(batches);
        }

        public async Task<TissBatchDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var batch = await _batchRepository.GetWithGuidesAsync(id, tenantId);
            return batch != null ? _mapper.Map<TissBatchDto>(batch) : null;
        }

        public async Task<IEnumerable<TissBatchDto>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            var batches = await _batchRepository.GetByClinicIdAsync(clinicId, tenantId);
            return _mapper.Map<IEnumerable<TissBatchDto>>(batches);
        }

        public async Task<IEnumerable<TissBatchDto>> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            var batches = await _batchRepository.GetByOperatorIdAsync(operatorId, tenantId);
            return _mapper.Map<IEnumerable<TissBatchDto>>(batches);
        }

        public async Task<IEnumerable<TissBatchDto>> GetByStatusAsync(string status, string tenantId)
        {
            if (!Enum.TryParse<BatchStatus>(status, true, out var statusEnum))
            {
                throw new ArgumentException($"Invalid status: {status}");
            }

            var batches = await _batchRepository.GetByStatusAsync(statusEnum, tenantId);
            return _mapper.Map<IEnumerable<TissBatchDto>>(batches);
        }

        public async Task<byte[]?> DownloadXmlAsync(Guid batchId, string tenantId)
        {
            var batch = await _batchRepository.GetByIdAsync(batchId, tenantId);
            if (batch == null || string.IsNullOrWhiteSpace(batch.XmlFilePath))
            {
                return null;
            }

            if (!File.Exists(batch.XmlFilePath))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(batch.XmlFilePath);
        }
    }
}
