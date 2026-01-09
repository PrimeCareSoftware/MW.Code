import SwiftUI

struct LoginView: View {
    @EnvironmentObject var authViewModel: AuthViewModel
    
    @State private var username = ""
    @State private var password = ""
    @State private var tenantId = ""
    @State private var isOwnerLogin = false
    @State private var showPassword = false
    
    // Animation states
    @State private var logoScale: CGFloat = 0.5
    @State private var logoOpacity: Double = 0
    @State private var formOffset: CGFloat = 50
    @State private var formOpacity: Double = 0
    @State private var backgroundAnimation: Bool = false
    
    // Focus states
    @FocusState private var focusedField: LoginField?
    
    enum LoginField {
        case username, password, tenantId
    }
    
    var body: some View {
        NavigationView {
            ZStack {
                // Animated gradient background
                AnimatedGradientBackground()
                
                // Decorative circles
                GeometryReader { geometry in
                    Circle()
                        .fill(Color.white.opacity(0.1))
                        .frame(width: 300, height: 300)
                        .blur(radius: 60)
                        .offset(x: -100, y: -100)
                        .scaleEffect(backgroundAnimation ? 1.2 : 1.0)
                    
                    Circle()
                        .fill(Color.mwAccent.opacity(0.15))
                        .frame(width: 250, height: 250)
                        .blur(radius: 50)
                        .offset(x: geometry.size.width - 100, y: geometry.size.height - 200)
                        .scaleEffect(backgroundAnimation ? 1.1 : 0.9)
                    
                    Circle()
                        .fill(Color.mwSecondary.opacity(0.1))
                        .frame(width: 200, height: 200)
                        .blur(radius: 40)
                        .offset(x: geometry.size.width / 2, y: 100)
                        .scaleEffect(backgroundAnimation ? 0.9 : 1.1)
                }
                .ignoresSafeArea()
                
                ScrollView(showsIndicators: false) {
                    VStack(spacing: MWDesign.Spacing.lg) {
                        // Logo and title with animation
                        VStack(spacing: MWDesign.Spacing.md) {
                            ZStack {
                                // Glow effect
                                Circle()
                                    .fill(Color.white.opacity(0.2))
                                    .frame(width: 120, height: 120)
                                    .blur(radius: 20)
                                
                                // Logo container
                                ZStack {
                                    Circle()
                                        .fill(.ultraThinMaterial)
                                        .frame(width: 100, height: 100)
                                    
                                    Image(systemName: "cross.case.fill")
                                        .font(.system(size: 45, weight: .medium))
                                        .foregroundStyle(
                                            LinearGradient(
                                                colors: [.white, .white.opacity(0.8)],
                                                startPoint: .top,
                                                endPoint: .bottom
                                            )
                                        )
                                }
                                .shadow(color: Color.mwPrimary.opacity(0.3), radius: 15, x: 0, y: 8)
                            }
                            .scaleEffect(logoScale)
                            .opacity(logoOpacity)
                            
                            VStack(spacing: MWDesign.Spacing.xxs) {
                                Text("PrimeCare Software")
                                    .font(.system(size: 32, weight: .bold, design: .rounded))
                                    .foregroundColor(.white)
                                
                                Text("Sistema de Gestão Médica")
                                    .font(.subheadline)
                                    .fontWeight(.medium)
                                    .foregroundColor(.white.opacity(0.85))
                            }
                            .opacity(logoOpacity)
                        }
                        .padding(.top, 60)
                        .padding(.bottom, MWDesign.Spacing.lg)
                        
                        // Login form with glassmorphism
                        VStack(spacing: MWDesign.Spacing.lg) {
                            // Username field
                            LoginTextField(
                                text: $username,
                                placeholder: "Digite seu usuário",
                                icon: "person.fill",
                                isSecure: false,
                                isFocused: focusedField == .username
                            )
                            .focused($focusedField, equals: .username)
                            .textInputAutocapitalization(.never)
                            .autocorrectionDisabled()
                            .submitLabel(.next)
                            .onSubmit { focusedField = .password }
                            
                            // Password field
                            LoginTextField(
                                text: $password,
                                placeholder: "Digite sua senha",
                                icon: "lock.fill",
                                isSecure: !showPassword,
                                isFocused: focusedField == .password,
                                trailingAction: {
                                    showPassword.toggle()
                                },
                                trailingIcon: showPassword ? "eye.slash.fill" : "eye.fill"
                            )
                            .focused($focusedField, equals: .password)
                            .submitLabel(.next)
                            .onSubmit { focusedField = .tenantId }
                            
                            // Tenant ID field
                            LoginTextField(
                                text: $tenantId,
                                placeholder: "Digite o ID da clínica",
                                icon: "building.2.fill",
                                isSecure: false,
                                isFocused: focusedField == .tenantId
                            )
                            .focused($focusedField, equals: .tenantId)
                            .textInputAutocapitalization(.never)
                            .autocorrectionDisabled()
                            .submitLabel(.done)
                            .onSubmit { focusedField = nil }
                            
                            // Owner login toggle
                            HStack {
                                Toggle("", isOn: $isOwnerLogin)
                                    .toggleStyle(MWToggleStyle())
                                    .labelsHidden()
                                
                                Text("Login como proprietário")
                                    .font(.subheadline)
                                    .fontWeight(.medium)
                                    .foregroundColor(.white)
                                
                                Spacer()
                            }
                            .padding(.top, MWDesign.Spacing.xs)
                            
                            // Error message
                            if let errorMessage = authViewModel.errorMessage {
                                HStack(spacing: MWDesign.Spacing.xs) {
                                    Image(systemName: "exclamationmark.triangle.fill")
                                        .font(.caption)
                                    Text(errorMessage)
                                        .font(.caption)
                                }
                                .foregroundColor(.mwError)
                                .padding(.horizontal, MWDesign.Spacing.md)
                                .padding(.vertical, MWDesign.Spacing.sm)
                                .frame(maxWidth: .infinity)
                                .background(Color.mwError.opacity(0.15))
                                .cornerRadius(MWDesign.Radius.sm)
                                .overlay(
                                    RoundedRectangle(cornerRadius: MWDesign.Radius.sm)
                                        .stroke(Color.mwError.opacity(0.3), lineWidth: 1)
                                )
                                .transition(.scale.combined(with: .opacity))
                            }
                            
                            // Login button
                            Button(action: performLogin) {
                                HStack(spacing: MWDesign.Spacing.sm) {
                                    if authViewModel.isLoading {
                                        ProgressView()
                                            .progressViewStyle(CircularProgressViewStyle(tint: .white))
                                            .scaleEffect(0.9)
                                    } else {
                                        Text("Entrar")
                                            .fontWeight(.semibold)
                                        
                                        Image(systemName: "arrow.right")
                                            .font(.body.weight(.semibold))
                                    }
                                }
                                .foregroundColor(.white)
                                .frame(maxWidth: .infinity)
                                .padding(.vertical, MWDesign.Spacing.md)
                                .background(
                                    Group {
                                        if isFormValid {
                                            LinearGradient(
                                                colors: [Color.mwPrimary, Color.mwPrimaryDark],
                                                startPoint: .leading,
                                                endPoint: .trailing
                                            )
                                        } else {
                                            Color.white.opacity(0.3)
                                        }
                                    }
                                )
                                .cornerRadius(MWDesign.Radius.md)
                                .shadow(
                                    color: isFormValid ? Color.mwPrimary.opacity(0.4) : Color.clear,
                                    radius: 12, x: 0, y: 6
                                )
                            }
                            .disabled(!isFormValid || authViewModel.isLoading)
                            .padding(.top, MWDesign.Spacing.sm)
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                        .padding(.vertical, MWDesign.Spacing.xl)
                        .background(.ultraThinMaterial)
                        .cornerRadius(MWDesign.Radius.xxl)
                        .overlay(
                            RoundedRectangle(cornerRadius: MWDesign.Radius.xxl)
                                .stroke(Color.white.opacity(0.2), lineWidth: 1)
                        )
                        .shadow(color: Color.black.opacity(0.15), radius: 20, x: 0, y: 10)
                        .padding(.horizontal, MWDesign.Spacing.lg)
                        .offset(y: formOffset)
                        .opacity(formOpacity)
                        
                        // Footer
                        VStack(spacing: MWDesign.Spacing.xs) {
                            Text("Versão 1.0.0")
                                .font(.caption2)
                                .foregroundColor(.white.opacity(0.6))
                        }
                        .padding(.top, MWDesign.Spacing.lg)
                        .opacity(formOpacity)
                        
                        Spacer(minLength: 40)
                    }
                }
            }
            .navigationBarHidden(true)
            .onAppear {
                startAnimations()
            }
        }
        .navigationViewStyle(.stack)
    }
    
    private var isFormValid: Bool {
        !username.isEmpty && !password.isEmpty && !tenantId.isEmpty
    }
    
    private func performLogin() {
        focusedField = nil
        Task {
            await authViewModel.login(
                username: username,
                password: password,
                tenantId: tenantId,
                isOwner: isOwnerLogin
            )
        }
    }
    
    private func startAnimations() {
        // Logo animation
        withAnimation(.spring(response: 0.8, dampingFraction: 0.7).delay(0.1)) {
            logoScale = 1.0
            logoOpacity = 1.0
        }
        
        // Form animation
        withAnimation(.spring(response: 0.8, dampingFraction: 0.8).delay(0.3)) {
            formOffset = 0
            formOpacity = 1.0
        }
        
        // Background animation
        withAnimation(.easeInOut(duration: 4).repeatForever(autoreverses: true)) {
            backgroundAnimation = true
        }
    }
}

// MARK: - Animated Gradient Background
struct AnimatedGradientBackground: View {
    @State private var animateGradient = false
    
    var body: some View {
        LinearGradient(
            colors: [
                Color.mwGradientStart,
                Color.mwGradientMiddle,
                Color.mwGradientEnd.opacity(0.8)
            ],
            startPoint: animateGradient ? .topLeading : .topTrailing,
            endPoint: animateGradient ? .bottomTrailing : .bottomLeading
        )
        .ignoresSafeArea()
        .onAppear {
            withAnimation(.easeInOut(duration: 5).repeatForever(autoreverses: true)) {
                animateGradient = true
            }
        }
    }
}

// MARK: - Custom Login Text Field
struct LoginTextField: View {
    @Binding var text: String
    let placeholder: String
    let icon: String
    var isSecure: Bool = false
    var isFocused: Bool = false
    var trailingAction: (() -> Void)? = nil
    var trailingIcon: String? = nil
    
    var body: some View {
        HStack(spacing: MWDesign.Spacing.sm) {
            Image(systemName: icon)
                .font(.body)
                .foregroundColor(isFocused ? .mwPrimary : .mwTextMuted)
                .frame(width: 24)
            
            Group {
                if isSecure {
                    SecureField(placeholder, text: $text)
                } else {
                    TextField(placeholder, text: $text)
                }
            }
            .font(.body)
            .foregroundColor(.mwTextPrimary)
            
            if let trailingIcon = trailingIcon {
                Button(action: { trailingAction?() }) {
                    Image(systemName: trailingIcon)
                        .font(.body)
                        .foregroundColor(.mwTextMuted)
                }
            }
        }
        .padding(.horizontal, MWDesign.Spacing.md)
        .padding(.vertical, MWDesign.Spacing.md + 2)
        .background(Color.white)
        .cornerRadius(MWDesign.Radius.md)
        .overlay(
            RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                .stroke(isFocused ? Color.mwPrimary : Color.mwBorder, lineWidth: isFocused ? 2 : 1)
        )
        .shadow(
            color: isFocused ? Color.mwPrimary.opacity(0.15) : Color.black.opacity(0.03),
            radius: isFocused ? 8 : 4,
            x: 0,
            y: 2
        )
        .animation(MWDesign.Animation.quick, value: isFocused)
    }
}

// MARK: - Custom Toggle Style
struct MWToggleStyle: ToggleStyle {
    func makeBody(configuration: Configuration) -> some View {
        HStack {
            configuration.label
            
            ZStack {
                Capsule()
                    .fill(configuration.isOn ? Color.mwAccent : Color.white.opacity(0.3))
                    .frame(width: 50, height: 28)
                    .overlay(
                        Capsule()
                            .stroke(Color.white.opacity(0.2), lineWidth: 1)
                    )
                
                Circle()
                    .fill(Color.white)
                    .frame(width: 22, height: 22)
                    .shadow(color: Color.black.opacity(0.15), radius: 2, x: 0, y: 1)
                    .offset(x: configuration.isOn ? 10 : -10)
                    .animation(.spring(response: 0.3, dampingFraction: 0.7), value: configuration.isOn)
            }
            .onTapGesture {
                configuration.isOn.toggle()
            }
        }
    }
}

#Preview {
    LoginView()
        .environmentObject(AuthViewModel())
}
