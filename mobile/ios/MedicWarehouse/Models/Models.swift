import Foundation

// MARK: - Date Formatters (Shared)

private let iso8601Formatter = ISO8601DateFormatter()
private let displayDateFormatter: DateFormatter = {
    let formatter = DateFormatter()
    formatter.dateStyle = .medium
    return formatter
}()

private let displayDateTimeFormatter: DateFormatter = {
    let formatter = DateFormatter()
    formatter.dateStyle = .medium
    formatter.timeStyle = .short
    return formatter
}()

// MARK: - Authentication Models

struct LoginRequest: Codable {
    let username: String
    let password: String
    let tenantId: String
}

struct LoginResponse: Codable {
    let token: String
    let username: String
    let role: String
    let tenantId: String
    let clinicId: String?
    let expiresAt: String
}

// MARK: - Patient Models

struct Patient: Codable, Identifiable {
    let id: String
    let fullName: String
    let cpf: String?
    let dateOfBirth: String?
    let phone: String?
    let email: String?
    let address: String?
    let medicalHistory: String?
    let allergies: String?
    let createdAt: String
    
    var displayDateOfBirth: String {
        guard let dob = dateOfBirth else { return "N/A" }
        if let date = iso8601Formatter.date(from: dob) {
            return displayDateFormatter.string(from: date)
        }
        return dob
    }
}

struct PatientListResponse: Codable {
    let items: [Patient]
    let totalCount: Int
    let pageNumber: Int
    let pageSize: Int
}

// MARK: - Appointment Models

struct Appointment: Codable, Identifiable {
    let id: String
    let patientId: String
    let patientName: String?
    let doctorId: String?
    let doctorName: String?
    let appointmentDate: String
    let appointmentTime: String
    let duration: Int
    let status: String
    let appointmentType: String?
    let notes: String?
    
    var displayDate: String {
        if let date = iso8601Formatter.date(from: appointmentDate) {
            return displayDateTimeFormatter.string(from: date)
        }
        return appointmentDate
    }
    
    var statusColor: String {
        switch status.lowercased() {
        case "scheduled": return "blue"
        case "in_progress": return "orange"
        case "completed": return "green"
        case "cancelled": return "red"
        default: return "gray"
        }
    }
}

struct AppointmentListResponse: Codable {
    let items: [Appointment]
    let totalCount: Int
    let pageNumber: Int
    let pageSize: Int
}

// MARK: - Dashboard Models

struct DashboardStats: Codable {
    let todayAppointments: Int
    let totalPatients: Int
    let pendingAppointments: Int
    let completedToday: Int
}

// MARK: - API Response Models

struct APIError: Codable {
    let message: String
    let code: String?
}

struct EmptyResponse: Codable {}
