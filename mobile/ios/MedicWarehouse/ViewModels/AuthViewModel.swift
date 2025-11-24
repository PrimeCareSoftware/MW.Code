import Foundation
import SwiftUI

@MainActor
class AuthViewModel: ObservableObject {
    @Published var isAuthenticated = false
    @Published var currentUser: LoginResponse?
    @Published var isLoading = false
    @Published var errorMessage: String?
    
    private let apiService = APIService()
    
    init() {
        checkAuthentication()
    }
    
    func checkAuthentication() {
        if NetworkManager.shared.getAuthToken() != nil {
            isAuthenticated = true
        }
    }
    
    func login(username: String, password: String, tenantId: String, isOwner: Bool = false) async {
        isLoading = true
        errorMessage = nil
        
        do {
            let response: LoginResponse
            if isOwner {
                response = try await apiService.ownerLogin(username: username, password: password, tenantId: tenantId)
            } else {
                response = try await apiService.login(username: username, password: password, tenantId: tenantId)
            }
            
            currentUser = response
            isAuthenticated = true
        } catch {
            errorMessage = error.localizedDescription
            isAuthenticated = false
        }
        
        isLoading = false
    }
    
    func logout() {
        apiService.logout()
        currentUser = nil
        isAuthenticated = false
    }
}
