import SwiftUI

struct AppointmentsView: View {
    @State private var appointments: [Appointment] = []
    @State private var isLoading = true
    @State private var selectedFilter: AppointmentFilter = .all
    @State private var errorMessage: String?
    
    private let apiService = APIService()
    
    enum AppointmentFilter: String, CaseIterable {
        case all = "Todos"
        case scheduled = "Agendados"
        case inProgress = "Em Andamento"
        case completed = "Concluídos"
        
        var apiValue: String? {
            switch self {
            case .all: return nil
            case .scheduled: return "scheduled"
            case .inProgress: return "in_progress"
            case .completed: return "completed"
            }
        }
    }
    
    var filteredAppointments: [Appointment] {
        if selectedFilter == .all {
            return appointments
        }
        return appointments.filter { $0.status.lowercased() == selectedFilter.apiValue }
    }
    
    var body: some View {
        NavigationView {
            VStack(spacing: 0) {
                // Filter Pills
                ScrollView(.horizontal, showsIndicators: false) {
                    HStack(spacing: 10) {
                        ForEach(AppointmentFilter.allCases, id: \.self) { filter in
                            FilterPill(
                                title: filter.rawValue,
                                isSelected: selectedFilter == filter
                            ) {
                                selectedFilter = filter
                            }
                        }
                    }
                    .padding(.horizontal)
                    .padding(.vertical, 10)
                }
                .background(Color(.systemGroupedBackground))
                
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
                                await loadAppointments()
                            }
                        }
                        .buttonStyle(.borderedProminent)
                    }
                    .padding()
                    Spacer()
                } else if filteredAppointments.isEmpty {
                    Spacer()
                    VStack(spacing: 15) {
                        Image(systemName: "calendar.badge.exclamationmark")
                            .font(.system(size: 50))
                            .foregroundColor(.gray)
                        
                        Text("Nenhum agendamento encontrado")
                            .foregroundColor(.gray)
                    }
                    Spacer()
                } else {
                    List(filteredAppointments) { appointment in
                        AppointmentRow(appointment: appointment)
                    }
                    .listStyle(PlainListStyle())
                }
            }
            .navigationTitle("Agendamentos")
            .toolbar {
                ToolbarItem(placement: .navigationBarTrailing) {
                    Button(action: {
                        // Add new appointment action
                    }) {
                        Image(systemName: "plus.circle.fill")
                            .font(.title3)
                    }
                }
            }
            .task {
                await loadAppointments()
            }
            .refreshable {
                await loadAppointments()
            }
        }
    }
    
    private func loadAppointments() async {
        isLoading = true
        errorMessage = nil
        
        do {
            let response = try await apiService.getAppointments(
                page: 1,
                pageSize: 100,
                status: selectedFilter.apiValue
            )
            appointments = response.items
        } catch {
            errorMessage = error.localizedDescription
        }
        
        isLoading = false
    }
}

struct FilterPill: View {
    let title: String
    let isSelected: Bool
    let action: () -> Void
    
    var body: some View {
        Button(action: action) {
            Text(title)
                .font(.subheadline)
                .fontWeight(isSelected ? .semibold : .regular)
                .foregroundColor(isSelected ? .white : .primary)
                .padding(.horizontal, 16)
                .padding(.vertical, 8)
                .background(isSelected ? Color.blue : Color(.systemGray6))
                .cornerRadius(20)
        }
    }
}

struct AppointmentRow: View {
    let appointment: Appointment
    
    var statusColor: Color {
        switch appointment.status.lowercased() {
        case "scheduled": return .blue
        case "in_progress": return .orange
        case "completed": return .green
        case "cancelled": return .red
        default: return .gray
        }
    }
    
    var statusText: String {
        switch appointment.status.lowercased() {
        case "scheduled": return "Agendado"
        case "in_progress": return "Em Andamento"
        case "completed": return "Concluído"
        case "cancelled": return "Cancelado"
        default: return appointment.status
        }
    }
    
    var body: some View {
        VStack(alignment: .leading, spacing: 12) {
            // Header with status
            HStack {
                HStack(spacing: 5) {
                    Circle()
                        .fill(statusColor)
                        .frame(width: 8, height: 8)
                    
                    Text(statusText)
                        .font(.caption)
                        .fontWeight(.semibold)
                        .foregroundColor(statusColor)
                }
                
                Spacer()
                
                Text(appointment.displayDate)
                    .font(.caption)
                    .foregroundColor(.gray)
            }
            
            // Patient name
            HStack {
                Image(systemName: "person.fill")
                    .foregroundColor(.blue)
                    .frame(width: 20)
                
                Text(appointment.patientName ?? "Paciente não informado")
                    .font(.headline)
            }
            
            // Doctor name (if available)
            if let doctorName = appointment.doctorName {
                HStack {
                    Image(systemName: "stethoscope")
                        .foregroundColor(.purple)
                        .frame(width: 20)
                    
                    Text(doctorName)
                        .font(.subheadline)
                        .foregroundColor(.gray)
                }
            }
            
            // Appointment type (if available)
            if let type = appointment.appointmentType {
                HStack {
                    Image(systemName: "tag.fill")
                        .foregroundColor(.orange)
                        .frame(width: 20)
                    
                    Text(type)
                        .font(.caption)
                        .foregroundColor(.gray)
                }
            }
            
            // Duration
            HStack {
                Image(systemName: "clock.fill")
                    .foregroundColor(.green)
                    .frame(width: 20)
                
                Text("\(appointment.duration) minutos")
                    .font(.caption)
                    .foregroundColor(.gray)
            }
        }
        .padding()
        .background(Color.white)
        .cornerRadius(12)
        .shadow(color: .black.opacity(0.05), radius: 5, x: 0, y: 2)
        .padding(.horizontal)
        .padding(.vertical, 5)
    }
}

#Preview {
    AppointmentsView()
}
