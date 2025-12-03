import SwiftUI

// MARK: - Custom Colors
extension Color {
    // Primary Brand Colors
    static let mwPrimary = Color(hex: "4F46E5") // Indigo 600
    static let mwPrimaryLight = Color(hex: "818CF8") // Indigo 400
    static let mwPrimaryDark = Color(hex: "3730A3") // Indigo 800
    
    // Secondary Colors
    static let mwSecondary = Color(hex: "8B5CF6") // Violet 500
    static let mwSecondaryLight = Color(hex: "A78BFA") // Violet 400
    
    // Accent Colors
    static let mwAccent = Color(hex: "06B6D4") // Cyan 500
    static let mwAccentLight = Color(hex: "22D3EE") // Cyan 400
    
    // Status Colors
    static let mwSuccess = Color(hex: "10B981") // Emerald 500
    static let mwWarning = Color(hex: "F59E0B") // Amber 500
    static let mwError = Color(hex: "EF4444") // Red 500
    static let mwInfo = Color(hex: "3B82F6") // Blue 500
    
    // Neutral Colors
    static let mwBackground = Color(hex: "F8FAFC") // Slate 50
    static let mwSurface = Color.white
    static let mwSurfaceSecondary = Color(hex: "F1F5F9") // Slate 100
    static let mwBorder = Color(hex: "E2E8F0") // Slate 200
    static let mwTextPrimary = Color(hex: "0F172A") // Slate 900
    static let mwTextSecondary = Color(hex: "64748B") // Slate 500
    static let mwTextMuted = Color(hex: "94A3B8") // Slate 400
    
    // Gradient Colors
    static let mwGradientStart = Color(hex: "4F46E5")
    static let mwGradientMiddle = Color(hex: "7C3AED")
    static let mwGradientEnd = Color(hex: "EC4899")
    
    init(hex: String) {
        let hex = hex.trimmingCharacters(in: CharacterSet.alphanumerics.inverted)
        var int: UInt64 = 0
        Scanner(string: hex).scanHexInt64(&int)
        let a, r, g, b: UInt64
        switch hex.count {
        case 3:
            (a, r, g, b) = (255, (int >> 8) * 17, (int >> 4 & 0xF) * 17, (int & 0xF) * 17)
        case 6:
            (a, r, g, b) = (255, int >> 16, int >> 8 & 0xFF, int & 0xFF)
        case 8:
            (a, r, g, b) = (int >> 24, int >> 16 & 0xFF, int >> 8 & 0xFF, int & 0xFF)
        default:
            (a, r, g, b) = (255, 0, 0, 0)
        }
        self.init(
            .sRGB,
            red: Double(r) / 255,
            green: Double(g) / 255,
            blue: Double(b) / 255,
            opacity: Double(a) / 255
        )
    }
}

// MARK: - Brand Gradients
extension LinearGradient {
    static let mwPrimaryGradient = LinearGradient(
        colors: [Color.mwGradientStart, Color.mwGradientMiddle, Color.mwGradientEnd],
        startPoint: .topLeading,
        endPoint: .bottomTrailing
    )
    
    static let mwSubtleGradient = LinearGradient(
        colors: [Color.mwPrimary.opacity(0.1), Color.mwSecondary.opacity(0.1)],
        startPoint: .topLeading,
        endPoint: .bottomTrailing
    )
    
    static let mwCardGradient = LinearGradient(
        colors: [Color.white, Color.mwSurfaceSecondary.opacity(0.5)],
        startPoint: .top,
        endPoint: .bottom
    )
    
    static let mwAccentGradient = LinearGradient(
        colors: [Color.mwAccent, Color.mwPrimary],
        startPoint: .leading,
        endPoint: .trailing
    )
}

// MARK: - Design Constants
struct MWDesign {
    // Spacing
    struct Spacing {
        static let xxs: CGFloat = 4
        static let xs: CGFloat = 8
        static let sm: CGFloat = 12
        static let md: CGFloat = 16
        static let lg: CGFloat = 24
        static let xl: CGFloat = 32
        static let xxl: CGFloat = 48
    }
    
    // Corner Radius
    struct Radius {
        static let sm: CGFloat = 8
        static let md: CGFloat = 12
        static let lg: CGFloat = 16
        static let xl: CGFloat = 20
        static let xxl: CGFloat = 28
        static let full: CGFloat = 100
    }
    
    // Shadows
    struct Shadow {
        static let sm = (color: Color.black.opacity(0.04), radius: CGFloat(4), x: CGFloat(0), y: CGFloat(2))
        static let md = (color: Color.black.opacity(0.06), radius: CGFloat(8), x: CGFloat(0), y: CGFloat(4))
        static let lg = (color: Color.black.opacity(0.08), radius: CGFloat(16), x: CGFloat(0), y: CGFloat(8))
        static let xl = (color: Color.black.opacity(0.12), radius: CGFloat(24), x: CGFloat(0), y: CGFloat(12))
        static let colored = (color: Color.mwPrimary.opacity(0.25), radius: CGFloat(16), x: CGFloat(0), y: CGFloat(8))
    }
    
    // Animation
    struct Animation {
        static let quick = SwiftUI.Animation.easeInOut(duration: 0.2)
        static let smooth = SwiftUI.Animation.easeInOut(duration: 0.3)
        static let bouncy = SwiftUI.Animation.spring(response: 0.4, dampingFraction: 0.6)
        static let gentle = SwiftUI.Animation.spring(response: 0.5, dampingFraction: 0.8)
    }
}

// MARK: - View Modifiers
struct MWCardStyle: ViewModifier {
    var padding: CGFloat = MWDesign.Spacing.md
    var cornerRadius: CGFloat = MWDesign.Radius.lg
    var shadowLevel: ShadowLevel = .medium
    
    enum ShadowLevel {
        case small, medium, large
    }
    
    func body(content: Content) -> some View {
        content
            .padding(padding)
            .background(Color.mwSurface)
            .cornerRadius(cornerRadius)
            .shadow(
                color: shadowColor,
                radius: shadowRadius,
                x: 0,
                y: shadowY
            )
    }
    
    private var shadowColor: Color {
        switch shadowLevel {
        case .small: return MWDesign.Shadow.sm.color
        case .medium: return MWDesign.Shadow.md.color
        case .large: return MWDesign.Shadow.lg.color
        }
    }
    
    private var shadowRadius: CGFloat {
        switch shadowLevel {
        case .small: return MWDesign.Shadow.sm.radius
        case .medium: return MWDesign.Shadow.md.radius
        case .large: return MWDesign.Shadow.lg.radius
        }
    }
    
    private var shadowY: CGFloat {
        switch shadowLevel {
        case .small: return MWDesign.Shadow.sm.y
        case .medium: return MWDesign.Shadow.md.y
        case .large: return MWDesign.Shadow.lg.y
        }
    }
}

struct MWGlassStyle: ViewModifier {
    var cornerRadius: CGFloat = MWDesign.Radius.xl
    
    func body(content: Content) -> some View {
        content
            .background(.ultraThinMaterial)
            .cornerRadius(cornerRadius)
            .overlay(
                RoundedRectangle(cornerRadius: cornerRadius)
                    .stroke(Color.white.opacity(0.2), lineWidth: 1)
            )
    }
}

struct MWPrimaryButtonStyle: ButtonStyle {
    @Environment(\.isEnabled) private var isEnabled
    
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .fontWeight(.semibold)
            .foregroundColor(.white)
            .frame(maxWidth: .infinity)
            .padding(.vertical, MWDesign.Spacing.md)
            .background(
                Group {
                    if isEnabled {
                        LinearGradient(
                            colors: [Color.mwPrimary, Color.mwPrimaryDark],
                            startPoint: .top,
                            endPoint: .bottom
                        )
                    } else {
                        Color.mwTextMuted
                    }
                }
            )
            .cornerRadius(MWDesign.Radius.md)
            .shadow(
                color: isEnabled ? Color.mwPrimary.opacity(0.3) : Color.clear,
                radius: configuration.isPressed ? 4 : 8,
                x: 0,
                y: configuration.isPressed ? 2 : 4
            )
            .scaleEffect(configuration.isPressed ? 0.98 : 1.0)
            .animation(MWDesign.Animation.quick, value: configuration.isPressed)
    }
}

struct MWSecondaryButtonStyle: ButtonStyle {
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .fontWeight(.semibold)
            .foregroundColor(.mwPrimary)
            .frame(maxWidth: .infinity)
            .padding(.vertical, MWDesign.Spacing.md)
            .background(Color.mwPrimary.opacity(0.1))
            .cornerRadius(MWDesign.Radius.md)
            .overlay(
                RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                    .stroke(Color.mwPrimary.opacity(0.3), lineWidth: 1)
            )
            .scaleEffect(configuration.isPressed ? 0.98 : 1.0)
            .animation(MWDesign.Animation.quick, value: configuration.isPressed)
    }
}

// MARK: - View Extensions
extension View {
    func mwCard(padding: CGFloat = MWDesign.Spacing.md, cornerRadius: CGFloat = MWDesign.Radius.lg, shadowLevel: MWCardStyle.ShadowLevel = .medium) -> some View {
        modifier(MWCardStyle(padding: padding, cornerRadius: cornerRadius, shadowLevel: shadowLevel))
    }
    
    func mwGlass(cornerRadius: CGFloat = MWDesign.Radius.xl) -> some View {
        modifier(MWGlassStyle(cornerRadius: cornerRadius))
    }
}

// MARK: - Custom Text Field Style
struct MWTextFieldStyle: TextFieldStyle {
    var iconName: String? = nil
    @Binding var isFocused: Bool
    
    init(iconName: String? = nil, isFocused: Binding<Bool> = .constant(false)) {
        self.iconName = iconName
        self._isFocused = isFocused
    }
    
    func _body(configuration: TextField<Self._Label>) -> some View {
        HStack(spacing: MWDesign.Spacing.sm) {
            if let iconName = iconName {
                Image(systemName: iconName)
                    .foregroundColor(isFocused ? .mwPrimary : .mwTextMuted)
                    .font(.body)
            }
            
            configuration
                .font(.body)
        }
        .padding(.horizontal, MWDesign.Spacing.md)
        .padding(.vertical, MWDesign.Spacing.md)
        .background(Color.mwSurface)
        .cornerRadius(MWDesign.Radius.md)
        .overlay(
            RoundedRectangle(cornerRadius: MWDesign.Radius.md)
                .stroke(isFocused ? Color.mwPrimary : Color.mwBorder, lineWidth: isFocused ? 2 : 1)
        )
        .shadow(color: isFocused ? Color.mwPrimary.opacity(0.1) : Color.clear, radius: 8, x: 0, y: 4)
        .animation(MWDesign.Animation.quick, value: isFocused)
    }
}

// MARK: - Shimmer Effect
struct ShimmerEffect: ViewModifier {
    @State private var phase: CGFloat = 0
    
    func body(content: Content) -> some View {
        content
            .overlay(
                GeometryReader { geometry in
                    Color.white
                        .opacity(0.3)
                        .mask(
                            Rectangle()
                                .fill(
                                    LinearGradient(
                                        gradient: Gradient(stops: [
                                            .init(color: .clear, location: 0),
                                            .init(color: .white, location: 0.5),
                                            .init(color: .clear, location: 1)
                                        ]),
                                        startPoint: .leading,
                                        endPoint: .trailing
                                    )
                                )
                                .rotationEffect(.degrees(30))
                                .offset(x: -geometry.size.width + phase * 2 * geometry.size.width)
                        )
                }
            )
            .onAppear {
                withAnimation(.linear(duration: 1.5).repeatForever(autoreverses: false)) {
                    phase = 1
                }
            }
    }
}

extension View {
    func shimmer() -> some View {
        modifier(ShimmerEffect())
    }
}

// MARK: - Loading Skeleton
struct SkeletonView: View {
    var height: CGFloat = 20
    var cornerRadius: CGFloat = MWDesign.Radius.sm
    
    var body: some View {
        Rectangle()
            .fill(Color.mwSurfaceSecondary)
            .frame(height: height)
            .cornerRadius(cornerRadius)
            .shimmer()
    }
}

// MARK: - Avatar View
struct MWAvatarView: View {
    let name: String
    var size: CGFloat = 48
    var gradient: LinearGradient = .mwPrimaryGradient
    
    private var initials: String {
        let words = name.split(separator: " ")
        if words.count >= 2 {
            return String(words[0].prefix(1) + words[1].prefix(1)).uppercased()
        }
        return String(name.prefix(2)).uppercased()
    }
    
    var body: some View {
        ZStack {
            Circle()
                .fill(gradient)
                .frame(width: size, height: size)
            
            Text(initials)
                .font(.system(size: size * 0.35, weight: .semibold, design: .rounded))
                .foregroundColor(.white)
        }
        .shadow(color: Color.mwPrimary.opacity(0.3), radius: 4, x: 0, y: 2)
    }
}

// MARK: - Status Badge
struct MWStatusBadge: View {
    let text: String
    let color: Color
    var style: BadgeStyle = .filled
    
    enum BadgeStyle {
        case filled, outlined, subtle
    }
    
    var body: some View {
        HStack(spacing: 4) {
            Circle()
                .fill(color)
                .frame(width: 6, height: 6)
            
            Text(text)
                .font(.caption)
                .fontWeight(.medium)
        }
        .padding(.horizontal, 10)
        .padding(.vertical, 5)
        .background(backgroundColor)
        .foregroundColor(foregroundColor)
        .cornerRadius(MWDesign.Radius.full)
        .overlay(
            RoundedRectangle(cornerRadius: MWDesign.Radius.full)
                .stroke(borderColor, lineWidth: style == .outlined ? 1 : 0)
        )
    }
    
    private var backgroundColor: Color {
        switch style {
        case .filled: return color
        case .outlined: return .clear
        case .subtle: return color.opacity(0.1)
        }
    }
    
    private var foregroundColor: Color {
        switch style {
        case .filled: return .white
        case .outlined: return color
        case .subtle: return color
        }
    }
    
    private var borderColor: Color {
        style == .outlined ? color : .clear
    }
}

// MARK: - Empty State View
struct MWEmptyStateView: View {
    let icon: String
    let title: String
    let message: String
    var actionTitle: String? = nil
    var action: (() -> Void)? = nil
    
    var body: some View {
        VStack(spacing: MWDesign.Spacing.lg) {
            ZStack {
                Circle()
                    .fill(LinearGradient.mwSubtleGradient)
                    .frame(width: 100, height: 100)
                
                Image(systemName: icon)
                    .font(.system(size: 40))
                    .foregroundStyle(.linearGradient(
                        colors: [.mwPrimary, .mwSecondary],
                        startPoint: .topLeading,
                        endPoint: .bottomTrailing
                    ))
            }
            
            VStack(spacing: MWDesign.Spacing.xs) {
                Text(title)
                    .font(.title3)
                    .fontWeight(.semibold)
                    .foregroundColor(.mwTextPrimary)
                
                Text(message)
                    .font(.body)
                    .foregroundColor(.mwTextSecondary)
                    .multilineTextAlignment(.center)
            }
            
            if let actionTitle = actionTitle, let action = action {
                Button(action: action) {
                    Text(actionTitle)
                }
                .buttonStyle(MWSecondaryButtonStyle())
                .frame(width: 200)
            }
        }
        .padding(MWDesign.Spacing.xl)
    }
}
