import SwiftUI

struct PatientsView: View {
    @State private var patients: [Patient] = []
    @State private var isLoading = true
    @State private var searchText = ""
    @State private var errorMessage: String?
    
    private let apiService = APIService()
    
    var filteredPatients: [Patient] {
        if searchText.isEmpty {
            return patients
        }
        return patients.filter { patient in
            patient.fullName.localizedCaseInsensitiveContains(searchText) ||
            (patient.cpf?.contains(searchText) ?? false) ||
            (patient.phone?.contains(searchText) ?? false)
        }
    }
    
    var body: some View {
        NavigationView {
            VStack {
                // Search bar
                HStack {
                    Image(systemName: "magnifyingglass")
                        .foregroundColor(.gray)
                    
                    TextField("Buscar pacientes...", text: $searchText)
                        .textFieldStyle(PlainTextFieldStyle())
                    
                    if !searchText.isEmpty {
                        Button(action: {
                            searchText = ""
                        }) {
                            Image(systemName: "xmark.circle.fill")
                                .foregroundColor(.gray)
                        }
                    }
                }
                .padding()
                .background(Color(.systemGray6))
                .cornerRadius(10)
                .padding(.horizontal)
                
                if isLoading {
                    Spacer()
                    ProgressView()
                        .scaleEffect(1.5)
                    Spacer()
                } else if let errorMessage = errorMessage {
                    Spacer()
                    VStack(spacing: 15) {
                        Image(systemName: "exclamationmark.triangle")
                            .font(.system(size: 50))
                            .foregroundColor(.orange)
                        
                        Text(errorMessage)
                            .multilineTextAlignment(.center)
                            .foregroundColor(.gray)
                        
                        Button("Tentar Novamente") {
                            Task {
                                await loadPatients()
                            }
                        }
                        .buttonStyle(.borderedProminent)
                    }
                    .padding()
                    Spacer()
                } else if filteredPatients.isEmpty {
                    Spacer()
                    VStack(spacing: 15) {
                        Image(systemName: "person.3.slash")
                            .font(.system(size: 50))
                            .foregroundColor(.gray)
                        
                        Text(searchText.isEmpty ? "Nenhum paciente cadastrado" : "Nenhum paciente encontrado")
                            .foregroundColor(.gray)
                    }
                    Spacer()
                } else {
                    List(filteredPatients) { patient in
                        PatientRow(patient: patient)
                    }
                    .listStyle(PlainListStyle())
                }
            }
            .navigationTitle("Pacientes")
            .toolbar {
                ToolbarItem(placement: .navigationBarTrailing) {
                    Button(action: {
                        // Add new patient action
                    }) {
                        Image(systemName: "plus.circle.fill")
                            .font(.title3)
                    }
                }
            }
            .task {
                await loadPatients()
            }
            .refreshable {
                await loadPatients()
            }
        }
    }
    
    private func loadPatients() async {
        isLoading = true
        errorMessage = nil
        
        do {
            let response = try await apiService.getPatients(page: 1, pageSize: 100)
            patients = response.items
        } catch {
            errorMessage = error.localizedDescription
        }
        
        isLoading = false
    }
}

struct PatientRow: View {
    let patient: Patient
    
    var body: some View {
        HStack(spacing: 15) {
            // Avatar
            Circle()
                .fill(LinearGradient(
                    gradient: Gradient(colors: [.blue, .purple]),
                    startPoint: .topLeading,
                    endPoint: .bottomTrailing
                ))
                .frame(width: 50, height: 50)
                .overlay(
                    Text(patient.fullName.prefix(1).uppercased())
                        .font(.title3)
                        .fontWeight(.bold)
                        .foregroundColor(.white)
                )
            
            // Patient Info
            VStack(alignment: .leading, spacing: 5) {
                Text(patient.fullName)
                    .font(.headline)
                
                if let cpf = patient.cpf {
                    Text("CPF: \(formatCPF(cpf))")
                        .font(.caption)
                        .foregroundColor(.gray)
                }
                
                if let phone = patient.phone {
                    HStack(spacing: 5) {
                        Image(systemName: "phone.fill")
                            .font(.caption)
                        Text(phone)
                            .font(.caption)
                    }
                    .foregroundColor(.blue)
                }
            }
            
            Spacer()
            
            Image(systemName: "chevron.right")
                .foregroundColor(.gray)
                .font(.caption)
        }
        .padding(.vertical, 8)
    }
    
    private func formatCPF(_ cpf: String) -> String {
        let cleaned = cpf.replacingOccurrences(of: "[^0-9]", with: "", options: .regularExpression)
        if cleaned.count == 11 {
            let mask = "XXX.XXX.XXX-XX"
            var result = ""
            var index = cleaned.startIndex
            for char in mask {
                if char == "X" {
                    result.append(cleaned[index])
                    index = cleaned.index(after: index)
                } else {
                    result.append(char)
                }
            }
            return result
        }
        return cpf
    }
}

#Preview {
    PatientsView()
}
