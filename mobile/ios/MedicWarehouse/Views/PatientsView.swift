import SwiftUI

struct PatientsView: View {
    @State private var patients: [Patient] = []
    @State private var isLoading = true
    @State private var searchText = ""
    @State private var errorMessage: String?
    @State private var isSearchFocused = false
    
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
            VStack(spacing: 0) {
                // Custom Header
                VStack(spacing: MWDesign.Spacing.md) {
                    HStack {
                        VStack(alignment: .leading, spacing: MWDesign.Spacing.xxs) {
                            Text("Pacientes")
                                .font(.system(size: 28, weight: .bold, design: .rounded))
                                .foregroundColor(.mwTextPrimary)
                            
                            Text("\(patients.count) cadastrados")
                                .font(.subheadline)
                                .foregroundColor(.mwTextSecondary)
                        }
                        
                        Spacer()
                        
                        Button(action: {
                            // Add new patient action
                        }) {
                            ZStack {
                                Circle()
                                    .fill(LinearGradient(colors: [Color.mwPrimary, Color.mwPrimaryLight], startPoint: .topLeading, endPoint: .bottomTrailing))
                                    .frame(width: 44, height: 44)
                                
                                Image(systemName: "plus")
                                    .font(.system(size: 18, weight: .semibold))
                                    .foregroundColor(.white)
                            }
                            .shadow(color: Color.mwPrimary.opacity(0.3), radius: 6, x: 0, y: 3)
                        }
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    .padding(.top, MWDesign.Spacing.md)
                    
                    // Enhanced search bar
                    HStack(spacing: MWDesign.Spacing.sm) {
                        HStack(spacing: MWDesign.Spacing.sm) {
                            Image(systemName: "magnifyingglass")
                                .font(.body)
                                .foregroundColor(isSearchFocused ? .mwPrimary : .mwTextMuted)
                            
                            TextField("Buscar por nome, CPF ou telefone...", text: $searchText)
                                .font(.body)
                                .foregroundColor(.mwTextPrimary)
                                .onTapGesture {
                                    withAnimation(MWDesign.Animation.quick) {
                                        isSearchFocused = true
                                    }
                                }
                            
                            if !searchText.isEmpty {
                                Button(action: {
                                    withAnimation(MWDesign.Animation.quick) {
                                        searchText = ""
                                    }
                                }) {
                                    Image(systemName: "xmark.circle.fill")
                                        .font(.body)
                                        .foregroundColor(.mwTextMuted)
                                }
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.md)
                        .padding(.vertical, MWDesign.Spacing.sm + 2)
                        .background(Color.mwSurface)
                        .cornerRadius(MWDesign.Radius.md)
                        .overlay(
                            RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                                .stroke(isSearchFocused ? Color.mwPrimary : Color.mwBorder, lineWidth: isSearchFocused ? 2 : 1)
                        )
                        .shadow(color: isSearchFocused ? Color.mwPrimary.opacity(0.1) : Color.black.opacity(0.02), radius: 6, x: 0, y: 2)
                    }
                    .padding(.horizontal, MWDesign.Spacing.lg)
                    .padding(.bottom, MWDesign.Spacing.sm)
                }
                .background(Color.mwBackground)
                
                // Content
                if isLoading {
                    VStack(spacing: MWDesign.Spacing.md) {
                        ForEach(0..<5) { _ in
                            SkeletonPatientRow()
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
                                await loadPatients()
                            }
                        }
                    )
                    Spacer()
                } else if filteredPatients.isEmpty {
                    Spacer()
                    MWEmptyStateView(
                        icon: searchText.isEmpty ? "person.3" : "magnifyingglass",
                        title: searchText.isEmpty ? "Nenhum paciente" : "Nenhum resultado",
                        message: searchText.isEmpty ? "Adicione seu primeiro paciente para começar" : "Tente buscar por outro termo"
                    )
                    Spacer()
                } else {
                    ScrollView(showsIndicators: false) {
                        LazyVStack(spacing: MWDesign.Spacing.sm) {
                            ForEach(Array(filteredPatients.enumerated()), id: \.element.id) { index, patient in
                                ModernPatientRow(patient: patient, delay: Double(index) * 0.05)
                            }
                        }
                        .padding(.horizontal, MWDesign.Spacing.lg)
                        .padding(.top, MWDesign.Spacing.md)
                        .padding(.bottom, MWDesign.Spacing.xl)
                    }
                    .refreshable {
                        await loadPatients()
                    }
                }
            }
            .background(Color.mwBackground)
            .navigationBarHidden(true)
            .task {
                await loadPatients()
            }
            .onTapGesture {
                isSearchFocused = false
                UIApplication.shared.sendAction(#selector(UIResponder.resignFirstResponder), to: nil, from: nil, for: nil)
            }
        }
        .navigationViewStyle(.stack)
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

// MARK: - Modern Patient Row
struct ModernPatientRow: View {
    let patient: Patient
    var delay: Double = 0
    
    @State private var appeared = false
    @State private var isPressed = false
    
    var body: some View {
        Button(action: {
            // Navigate to patient details
        }) {
            HStack(spacing: MWDesign.Spacing.md) {
                // Avatar
                MWAvatarView(name: patient.fullName, size: 52)
                
                // Patient Info
                VStack(alignment: .leading, spacing: MWDesign.Spacing.xxs) {
                    Text(patient.fullName)
                        .font(.body)
                        .fontWeight(.semibold)
                        .foregroundColor(.mwTextPrimary)
                        .lineLimit(1)
                    
                    if let cpf = patient.cpf {
                        HStack(spacing: MWDesign.Spacing.xxs) {
                            Image(systemName: "person.text.rectangle")
                                .font(.caption2)
                            Text(formatCPF(cpf))
                                .font(.caption)
                        }
                        .foregroundColor(.mwTextSecondary)
                    }
                    
                    if let phone = patient.phone {
                        HStack(spacing: MWDesign.Spacing.xxs) {
                            Image(systemName: "phone.fill")
                                .font(.caption2)
                            Text(phone)
                                .font(.caption)
                        }
                        .foregroundColor(.mwPrimary)
                    }
                }
                
                Spacer()
                
                // Arrow indicator
                ZStack {
                    Circle()
                        .fill(Color.mwSurfaceSecondary)
                        .frame(width: 32, height: 32)
                    
                    Image(systemName: "chevron.right")
                        .font(.caption)
                        .fontWeight(.semibold)
                        .foregroundColor(.mwTextMuted)
                }
            }
            .padding(MWDesign.Spacing.md)
            .background(Color.mwSurface)
            .cornerRadius(MWDesign.Radius.lg)
            .shadow(color: Color.black.opacity(0.03), radius: 6, x: 0, y: 2)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.lg)
                    .stroke(Color.mwBorder.opacity(0.5), lineWidth: 1)
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

// MARK: - Skeleton Patient Row
struct SkeletonPatientRow: View {
    var body: some View {
        HStack(spacing: MWDesign.Spacing.md) {
            Circle()
                .fill(Color.mwSurfaceSecondary)
                .frame(width: 52, height: 52)
            
            VStack(alignment: .leading, spacing: MWDesign.Spacing.xs) {
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 140, height: 16)
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 100, height: 12)
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.mwSurfaceSecondary)
                    .frame(width: 80, height: 12)
            }
            
            Spacer()
            
            Circle()
                .fill(Color.mwSurfaceSecondary)
                .frame(width: 32, height: 32)
        }
        .padding(MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.lg)
        .shimmer()
    }
}

#Preview {
    PatientsView()
}
