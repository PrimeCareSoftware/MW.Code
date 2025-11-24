import SwiftUI

struct LoginView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    
    @State private var username = ""
    @State private var password = ""
    @State private var tenantId = ""
    @State private var isOwnerLogin = false
    
    var body: some View {
        NavigationView {
            ZStack {
                // Gradient background
                LinearGradient(
                    gradient: Gradient(colors: [Color.blue.opacity(0.6), Color.purple.opacity(0.6)]),
                    startPoint: .topLeading,
                    endPoint: .bottomTrailing
                )
                .ignoresSafeArea()
                
                ScrollView {
                    VStack(spacing: 25) {
                        // Logo and title
                        VStack(spacing: 10) {
                            Image(systemName: "cross.case.fill")
                                .font(.system(size: 80))
                                .foregroundColor(.white)
                            
                            Text("MedicWarehouse")
                                .font(.largeTitle)
                                .fontWeight(.bold)
                                .foregroundColor(.white)
                            
                            Text("Sistema de Gestão Médica")
                                .font(.subheadline)
                                .foregroundColor(.white.opacity(0.9))
                        }
                        .padding(.top, 60)
                        .padding(.bottom, 30)
                        
                        // Login form
                        VStack(spacing: 20) {
                            // Username
                            VStack(alignment: .leading, spacing: 8) {
                                Text("Usuário")
                                    .foregroundColor(.white)
                                    .fontWeight(.semibold)
                                
                                TextField("Digite seu usuário", text: $username)
                                    .textFieldStyle(RoundedTextFieldStyle())
                                    .textInputAutocapitalization(.never)
                                    .autocorrectionDisabled()
                            }
                            
                            // Password
                            VStack(alignment: .leading, spacing: 8) {
                                Text("Senha")
                                    .foregroundColor(.white)
                                    .fontWeight(.semibold)
                                
                                SecureField("Digite sua senha", text: $password)
                                    .textFieldStyle(RoundedTextFieldStyle())
                            }
                            
                            // Tenant ID
                            VStack(alignment: .leading, spacing: 8) {
                                Text("ID da Clínica")
                                    .foregroundColor(.white)
                                    .fontWeight(.semibold)
                                
                                TextField("Digite o ID da clínica", text: $tenantId)
                                    .textFieldStyle(RoundedTextFieldStyle())
                                    .textInputAutocapitalization(.never)
                                    .autocorrectionDisabled()
                            }
                            
                            // Owner login toggle
                            Toggle(isOn: $isOwnerLogin) {
                                Text("Login como proprietário")
                                    .foregroundColor(.white)
                                    .fontWeight(.semibold)
                            }
                            .toggleStyle(SwitchToggleStyle(tint: .white))
                            
                            // Error message
                            if let errorMessage = authViewModel.errorMessage {
                                Text(errorMessage)
                                    .foregroundColor(.red)
                                    .font(.caption)
                                    .padding(.horizontal)
                                    .padding(.vertical, 8)
                                    .background(Color.white.opacity(0.9))
                                    .cornerRadius(8)
                            }
                            
                            // Login button
                            Button(action: {
                                Task {
                                    await authViewModel.login(
                                        username: username,
                                        password: password,
                                        tenantId: tenantId,
                                        isOwner: isOwnerLogin
                                    )
                                }
                            }) {
                                if authViewModel.isLoading {
                                    ProgressView()
                                        .progressViewStyle(CircularProgressViewStyle(tint: .blue))
                                } else {
                                    Text("Entrar")
                                        .fontWeight(.bold)
                                        .foregroundColor(.blue)
                                }
                            }
                            .frame(maxWidth: .infinity)
                            .padding()
                            .background(Color.white)
                            .cornerRadius(12)
                            .disabled(authViewModel.isLoading || username.isEmpty || password.isEmpty || tenantId.isEmpty)
                        }
                        .padding(.horizontal, 30)
                        .padding(.vertical, 40)
                        .background(Color.white.opacity(0.15))
                        .cornerRadius(20)
                        .padding(.horizontal, 20)
                        
                        Spacer()
                    }
                }
            }
            .navigationBarHidden(true)
        }
    }
}

// Custom TextField Style
struct RoundedTextFieldStyle: TextFieldStyle {
    func _body(configuration: TextField<Self._Label>) -> some View {
        configuration
            .padding()
            .background(Color.white)
            .cornerRadius(10)
            .shadow(color: .black.opacity(0.1), radius: 5, x: 0, y: 2)
    }
}

#Preview {
    LoginView()
        .environmentObject(AuthViewModel())
}
