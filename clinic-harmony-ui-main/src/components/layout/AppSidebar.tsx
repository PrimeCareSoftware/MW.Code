import { NavLink, useLocation } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  Calendar,
  FileText,
  Settings,
  Package,
  Building2,
  Shield,
  ChevronLeft,
  ChevronRight,
  Stethoscope,
  ClipboardList,
  BarChart3,
  Bell,
  X,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { useSidebar } from "@/contexts/SidebarContext";

interface NavItem {
  title: string;
  icon: React.ElementType;
  path: string;
  badge?: number;
}

interface AppSidebarProps {
  variant?: "medicwarehouse" | "patient-portal" | "admin";
}

const navigationConfigs: Record<string, NavItem[]> = {
  medicwarehouse: [
    { title: "Dashboard", icon: LayoutDashboard, path: "/" },
    { title: "Pacientes", icon: Users, path: "/patients" },
    { title: "Consultas", icon: Calendar, path: "/appointments", badge: 3 },
    { title: "Procedimentos", icon: Stethoscope, path: "/procedures" },
    { title: "Prontuários", icon: FileText, path: "/records" },
    { title: "Inventário", icon: Package, path: "/inventory" },
    { title: "Relatórios", icon: BarChart3, path: "/reports" },
    { title: "Configurações", icon: Settings, path: "/settings" },
  ],
  "patient-portal": [
    { title: "Início", icon: LayoutDashboard, path: "/patient-portal" },
    { title: "Minhas Consultas", icon: Calendar, path: "/my-appointments" },
    { title: "Meus Exames", icon: ClipboardList, path: "/my-exams" },
    { title: "Prontuário", icon: FileText, path: "/my-records" },
    { title: "Notificações", icon: Bell, path: "/notifications", badge: 2 },
    { title: "Configurações", icon: Settings, path: "/settings" },
  ],
  admin: [
    { title: "Dashboard", icon: LayoutDashboard, path: "/admin" },
    { title: "Clínicas", icon: Building2, path: "/clinics" },
    { title: "Usuários", icon: Users, path: "/users" },
    { title: "Permissões", icon: Shield, path: "/permissions" },
    { title: "Relatórios", icon: BarChart3, path: "/reports" },
    { title: "Sistema", icon: Settings, path: "/system" },
  ],
};

const systemLabels: Record<string, string> = {
  medicwarehouse: "MedicWarehouse",
  "patient-portal": "Portal do Paciente",
  admin: "System Admin",
};

export function AppSidebar({ variant = "medicwarehouse" }: AppSidebarProps) {
  const { collapsed, setCollapsed, isMobile, mobileOpen, setMobileOpen } = useSidebar();
  const location = useLocation();
  const navigation = navigationConfigs[variant];
  const systemLabel = systemLabels[variant];

  const isOpen = isMobile ? mobileOpen : true;
  const sidebarWidth = collapsed && !isMobile ? "w-[72px]" : "w-64";

  if (isMobile && !mobileOpen) {
    return null;
  }

  return (
    <>
      {/* Mobile Overlay */}
      {isMobile && mobileOpen && (
        <div
          className="fixed inset-0 z-30 bg-black/50 transition-opacity"
          onClick={() => setMobileOpen(false)}
        />
      )}

      <aside
        className={cn(
          "fixed left-0 top-0 z-40 h-screen bg-sidebar border-r border-sidebar-border transition-apple",
          sidebarWidth,
          isMobile && "shadow-2xl"
        )}
      >
        {/* Logo Area */}
        <div className="flex h-16 items-center justify-between px-4 border-b border-sidebar-border">
          {(!collapsed || isMobile) && (
            <div className="flex items-center gap-3">
              <div className="h-8 w-8 rounded-lg bg-primary flex items-center justify-center">
                <Stethoscope className="h-4 w-4 text-primary-foreground" />
              </div>
              <span className="font-semibold text-sidebar-foreground">{systemLabel}</span>
            </div>
          )}
          {collapsed && !isMobile && (
            <div className="h-8 w-8 rounded-lg bg-primary flex items-center justify-center mx-auto">
              <Stethoscope className="h-4 w-4 text-primary-foreground" />
            </div>
          )}
          {isMobile && (
            <button
              onClick={() => setMobileOpen(false)}
              className="flex h-8 w-8 items-center justify-center rounded-lg text-muted-foreground hover:bg-secondary transition-apple"
            >
              <X className="h-5 w-5" />
            </button>
          )}
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-4">
          <ul className="space-y-1 px-3">
            {navigation.map((item) => {
              const isActive = location.pathname === item.path;
              return (
                <li key={item.path}>
                  <NavLink
                    to={item.path}
                    onClick={() => isMobile && setMobileOpen(false)}
                    className={cn(
                      "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-apple",
                      isActive
                        ? "bg-sidebar-accent text-sidebar-primary"
                        : "text-sidebar-foreground hover:bg-sidebar-accent/50"
                    )}
                  >
                    <item.icon className={cn("h-5 w-5 shrink-0", isActive && "text-sidebar-primary")} />
                    {(!collapsed || isMobile) && (
                      <>
                        <span className="flex-1">{item.title}</span>
                        {item.badge && (
                          <span className="flex h-5 min-w-5 items-center justify-center rounded-full bg-primary px-1.5 text-xs font-medium text-primary-foreground">
                            {item.badge}
                          </span>
                        )}
                      </>
                    )}
                  </NavLink>
                </li>
              );
            })}
          </ul>
        </nav>

        {/* Collapse Toggle - Only on Desktop */}
        {!isMobile && (
          <div className="absolute -right-3 top-20">
            <button
              onClick={() => setCollapsed(!collapsed)}
              className="flex h-6 w-6 items-center justify-center rounded-full border border-border bg-card shadow-apple-sm transition-apple hover:bg-secondary"
            >
              {collapsed ? (
                <ChevronRight className="h-3.5 w-3.5 text-muted-foreground" />
              ) : (
                <ChevronLeft className="h-3.5 w-3.5 text-muted-foreground" />
              )}
            </button>
          </div>
        )}
      </aside>
    </>
  );
}
