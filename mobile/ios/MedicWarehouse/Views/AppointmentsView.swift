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
        
        var icon: String {
            switch self {
            case .all: return "list.bullet"
            case .scheduled: return "calendar.badge.clock"
            case .inProgress: return "clock.fill"
            case .completed: return "checkmark.circle.fill"
            }
        }
        
        var color: Color {
            switch self {
            case .all: return .mwTextSecondary
            case .scheduled: return .mwPrimary
            case .inProgress: return .mwWarning
            case .completed: return .mwSuccess
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
                // Custom Header
                VStack(spacing: MWDesign.Spacing.md) {
                    HStack {
                        VStack(alignment: .leading, spacing: MWDesign.Spacing.xxs) {
                            Text("Agendamentos")
                                .font(.system(size: 28, weight: .bold, design: .rounded))
                                .foregroundColor(.mwTextPrimary)
                            
                            Text("\(appointments.count) no total")
                                .font(.subheadline)
                                .foregroundColor(.mwTextSecondary)
                        }
                        
                        Spacer()
                        
                        Button(action: {
                            // Add new appointment action
                        }) {
                            ZStack {
                                Circle()
                                    .fill(LinearGradient(colors: [Color.mwSuccess, Color(hex: "34D399")], startPoint: .topLeading, endPoint: .bottomTrailing))
                                    .frame(width: 44, height: 44)
                                
                                Image(systemName: "plus")
                                    .font(.system(size: 18, weight: .semibold))
                                    .foregroundColor(.white)
                            }
                            .shadow(color: Color.mwSuccess.opacity(0.3), radius: 6, x: 0, y: 3)
                        }
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    .padding(.top, MWDesign.Spacing.md)
                    
                    // Enhanced Filter Pills
                    ScrollView(.horizontal, showsIndicators: false) {
                        HStack(spacing: MWDesign.Spacing.sm) {
                            ForEach(AppointmentFilter.allCases, id: \.self) { filter in
                                ModernFilterPill(
                                    title: filter.rawValue,
                                    icon: filter.icon,
                                    color: filter.color,
                                    isSelected: selectedFilter == filter
                                ) {
                                    withAnimation(MWDesign.Animation.smooth) {
                                        selectedFilter = filter
                                    }
                                }
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                    }
                    .padding(.bottom, MWDesign.Spacing.sm)
                }
                .background(Color.mwBackground)
                
                // Content
                if isLoading {
                    VStack(spacing: MWDesign.Spacing.md) {
                        ForEach(0..<4) { _ in
                            SkeletonAppointmentCard()
                        }
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    .padding(.top, MWDesign.Spacing.md)
                    
                    Spacer()
                } else if let errorMessage = errorMessage {
                    Spacer()
                    MWEmptyStateView(
                        icon: "exclamationmark.triangle",
                        title: "Erro ao carregar",
                        message: errorMessage,
                        actionTitle: "Tentar Novamente",
                        action: {
                            Task {
                                await loadAppointments()
                            }
                        }
                    )
                    Spacer()
                } else if filteredAppointments.isEmpty {
                    Spacer()
                    MWEmptyStateView(
                        icon: "calendar.badge.exclamationmark",
                        title: "Nenhum agendamento",
                        message: selectedFilter == .all ? "Não há agendamentos registrados" : "Nenhum agendamento com status \"\(selectedFilter.rawValue)\""
                    )
                    Spacer()
                } else {
                    ScrollView(showsIndicators: false) {
                        LazyVStack(spacing: MWDesign.Spacing.md) {
                            ForEach(Array(filteredAppointments.enumerated()), id: \.element.id) { index, appointment in
                                ModernAppointmentCard(appointment: appointment, delay: Double(index) * 0.05)
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                        .padding(.top, MWDesign.Spacing.md)
                        .padding(.bottom, MWDesign.Spacing.xl)
                    }
                    .refreshable {
                        await loadAppointments()
                    }
                }
            }
            .background(Color.mwBackground)
            .navigationBarHidden(true)
            .task {
                await loadAppointments()
            }
        }
        .navigationViewStyle(.stack)
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

// MARK: - Modern Filter Pill
struct ModernFilterPill: View {
    let title: String
    let icon: String
    let color: Color
    let isSelected: Bool
    let action: () -> Void
    
    var body: some View {
        Button(action: action) {
            HStack(spacing: MWDesign.Spacing.xs) {
                Image(systemName: icon)
                    .font(.caption)
                    .fontWeight(.medium)
                
                Text(title)
                    .font(.subheadline)
                    .fontWeight(isSelected ? .semibold : .medium)
            }
            .foregroundColor(isSelected ? .white : color)
            .padding(.horizontal, MWDesign.Spacing.md)
            .padding(.vertical, MWDesign.Spacing.sm)
            .background(
                Group {
                    if isSelected {
                        LinearGradient(colors: [color, color.opacity(0.8)], startPoint: .leading, endPoint: .trailing)
                    } else {
                        color.opacity(0.1)
                    }
                }
            )
            .cornerRadius(MWDesign.Radius.full)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.full)
                    .stroke(isSelected ? Color.clear : color.opacity(0.2), lineWidth: 1)
            )
            .shadow(color: isSelected ? color.opacity(0.3) : Color.clear, radius: 4, x: 0, y: 2)
        }
    }
}

// MARK: - Modern Appointment Card
struct ModernAppointmentCard: View {
    let appointment: Appointment
    var delay: Double = 0
    
    @State private var appeared = false
    @State private var isPressed = false
    
    var statusColor: Color {
        switch appointment.status.lowercased() {
        case "scheduled": return .mwPrimary
        case "in_progress": return .mwWarning
        case "completed": return .mwSuccess
        case "cancelled": return .mwError
        default: return .mwTextMuted
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
    
    var statusIcon: String {
        switch appointment.status.lowercased() {
        case "scheduled": return "calendar.badge.clock"
        case "in_progress": return "clock.fill"
        case "completed": return "checkmark.circle.fill"
        case "cancelled": return "xmark.circle.fill"
        default: return "questionmark.circle"
        }
    }
    
    var body: some View {
        Button(action: {
            // Navigate to appointment details
        }) {
            VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
                // Header with status and date
                HStack {
                    MWStatusBadge(text: statusText, color: statusColor, style: .subtle)
                    
                    Spacer()
                    
                    HStack(spacing: MWDesign.Spacing.xxs) {
                        Image(systemName: "calendar")
                            .font(.caption)
                        Text(appointment.displayDate)
                            .font(.caption)
                            .fontWeight(.medium)
                    }
                    .foregroundColor(.mwTextSecondary)
                }
                
                Divider()
                    .background(Color.mwBorder)
                
                // Patient info
                HStack(spacing: MWDesign.Spacing.sm) {
                    MWAvatarView(
                        name: appointment.patientName ?? "P",
                        size: 44,
                        gradient: LinearGradient(colors: [.mwPrimary, .mwSecondary], startPoint: .topLeading, endPoint: .bottomTrailing)
                    )
                    
                    VStack(alignment: .leading, spacing: 2) {
                        Text(appointment.patientName ?? "Paciente não informado")
                            .font(.body)
                            .fontWeight(.semibold)
                            .foregroundColor(.mwTextPrimary)
                            .lineLimit(1)
                        
                        if let doctorName = appointment.doctorName {
                            HStack(spacing: MWDesign.Spacing.xxs) {
                                Image(systemName: "stethoscope")
                                    .font(.caption2)
                                Text(doctorName)
                                    .font(.caption)
                            }
                            .foregroundColor(.mwTextSecondary)
                        }
                    }
                    
                    Spacer()
                }
                
                // Additional info
                HStack(spacing: MWDesign.Spacing.lg) {
                    // Duration
                    HStack(spacing: MWDesign.Spacing.xxs) {
                        Image(systemName: "clock")
                            .font(.caption)
                            .foregroundColor(.mwAccent)
                        Text("\(appointment.duration) min")
                            .font(.caption)
                            .fontWeight(.medium)
                            .foregroundColor(.mwTextSecondary)
                    }
                    
                    // Appointment type
                    if let type = appointment.appointmentType {
                        HStack(spacing: MWDesign.Spacing.xxs) {
                            Image(systemName: "tag.fill")
                                .font(.caption)
                                .foregroundColor(.mwWarning)
                            Text(type)
                                .font(.caption)
                                .fontWeight(.medium)
                                .foregroundColor(.mwTextSecondary)
                                .lineLimit(1)
                        }
                    }
                    
                    Spacer()
                    
                    // Arrow
                    Image(systemName: "chevron.right")
                        .font(.caption)
                        .fontWeight(.semibold)
                        .foregroundColor(.mwTextMuted)
                }
            }
            .padding(MWDesign.Spacing.md)
            .background(Color.mwSurface)
            .cornerRadius(MWDesign.Radius.lg)
            .shadow(color: Color.black.opacity(0.04), radius: 8, x: 0, y: 4)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.lg)
                    .stroke(Color.mwBorder.opacity(0.5), lineWidth: 1)
            )
            .overlay(
                // Left accent bar
                HStack {
                    RoundedRectangle(cornerRadius: 2)
                        .fill(statusColor)
                        .frame(width: 4)
                    Spacer()
                }
                .padding(.vertical, MWDesign.Spacing.sm)
            )
            .scaleEffect(isPressed ? 0.98 : 1.0)
        }
        .buttonStyle(PlainButtonStyle())
        .simultaneousGesture(
            DragGesture(minimumDistance: 0)
                .onChanged { _ in
                    withAnimation(MWDesign.Animation.quick) {
                        isPressed = true
                    }
                }
                .onEnded { _ in
                    withAnimation(MWDesign.Animation.quick) {
                        isPressed = false
                    }
                }
        )
        .opacity(appeared ? 1.0 : 0)
        .offset(y: appeared ? 0 : 20)
        .onAppear {
            withAnimation(.spring(response: 0.4, dampingFraction: 0.8).delay(delay)) {
                appeared = true
            }
        }
    }
}

// MARK: - Skeleton Appointment Card
struct SkeletonAppointmentCard: View {
    var body: some View {
        VStack(alignment: .leading, spacing: MWDesign.Spacing.md) {
            HStack {
                RoundedRectangle(cornerRadius: MWDesign.Radius.full)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 80, height: 24)
                
                Spacer()
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 70, height: 14)
            }
            
            Divider()
            
            HStack(spacing: MWDesign.Spacing.sm) {
                Circle()
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 44, height: 44)
                
                VStack(alignment: .leading, spacing: MWDesign.Spacing.xs) {
                    RoundedRectangle(cornerRadius: 4)
                        .fill(Color.mwSurfaceSecondary)
                        .frame(width: 140, height: 16)
                    
                    RoundedRectangle(cornerRadius: 4)
                        .fill(Color.mwSurfaceSecondary)
                        .frame(width: 100, height: 12)
                }
                
                Spacer()
            }
            
            HStack(spacing: MWDesign.Spacing.lg) {
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 60, height: 14)
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 80, height: 14)
                
                Spacer()
            }
        }
        .padding(MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.lg)
        .shimmer()
    }
}

#Preview {
    AppointmentsView()
}
