import Foundation

class APIService {
    private let networkManager = NetworkManager.shared
    
    // MARK: - Authentication
    
    func login(username: String, password: String, tenantId: String) async throws -> LoginResponse {
        let request = LoginRequest(username: username, password: password, tenantId: tenantId)
        let response: LoginResponse = try await networkManager.request(
            endpoint: "/auth/login",
            method: "POST",
            body: request,
            authenticated: false
        )
        networkManager.setAuthToken(response.token)
        return response
    }
    
    func ownerLogin(username: String, password: String, tenantId: String) async throws -> LoginResponse {
        let request = LoginRequest(username: username, password: password, tenantId: tenantId)
        let response: LoginResponse = try await networkManager.request(
            endpoint: "/auth/owner-login",
            method: "POST",
            body: request,
            authenticated: false
        )
        networkManager.setAuthToken(response.token)
        return response
    }
    
    func logout() {
        networkManager.setAuthToken(nil)
    }
    
    // MARK: - Patients
    
    func getPatients(page: Int = 1, pageSize: Int = 20, searchTerm: String? = nil) async throws -> PatientListResponse {
        var endpoint = "/patients?page=\(page)&pageSize=\(pageSize)"
        if let searchTerm = searchTerm, !searchTerm.isEmpty {
            endpoint += "&searchTerm=\(searchTerm.addingPercentEncoding(withAllowedCharacters: .urlQueryAllowed) ?? "")"
        }
        return try await networkManager.request(endpoint: endpoint)
    }
    
    func getPatient(id: String) async throws -> Patient {
        return try await networkManager.request(endpoint: "/patients/\(id)")
    }
    
    func searchPatients(searchTerm: String) async throws -> [Patient] {
        let endpoint = "/patients/search?searchTerm=\(searchTerm.addingPercentEncoding(withAllowedCharacters: .urlQueryAllowed) ?? "")"
        return try await networkManager.request(endpoint: endpoint)
    }
    
    // MARK: - Appointments
    
    func getAppointments(page: Int = 1, pageSize: Int = 20, status: String? = nil) async throws -> AppointmentListResponse {
        var endpoint = "/appointments?page=\(page)&pageSize=\(pageSize)"
        if let status = status {
            endpoint += "&status=\(status)"
        }
        return try await networkManager.request(endpoint: endpoint)
    }
    
    func getAppointment(id: String) async throws -> Appointment {
        return try await networkManager.request(endpoint: "/appointments/\(id)")
    }
    
    func getTodayAppointments() async throws -> [Appointment] {
        return try await networkManager.request(endpoint: "/appointments/agenda")
    }
    
    // MARK: - Dashboard
    
    func getDashboardStats() async throws -> DashboardStats {
        // This would need to be implemented on the backend
        // For now, we'll aggregate data from appointments and patients
        let appointments = try await getTodayAppointments()
        let patients = try await getPatients(page: 1, pageSize: 1)
        
        let completed = appointments.filter { $0.status.lowercased() == "completed" }.count
        let pending = appointments.filter { $0.status.lowercased() == "scheduled" }.count
        
        return DashboardStats(
            todayAppointments: appointments.count,
            totalPatients: patients.totalCount,
            pendingAppointments: pending,
            completedToday: completed
        )
    }
}
