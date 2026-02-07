import { AppSidebar } from "./AppSidebar";
import { Topbar } from "./Topbar";
import { cn } from "@/lib/utils";
import { SidebarProvider, useSidebar } from "@/contexts/SidebarContext";

interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface DashboardLayoutProps {
  children: React.ReactNode;
  breadcrumbs?: BreadcrumbItem[];
  variant?: "medicwarehouse" | "patient-portal" | "admin";
}

function DashboardContent({
  children,
  breadcrumbs = [],
  variant = "medicwarehouse",
}: DashboardLayoutProps) {
  const { collapsed, isMobile } = useSidebar();

  return (
    <div className="min-h-screen bg-background">
      <AppSidebar variant={variant} />
      <Topbar breadcrumbs={breadcrumbs} />
      <main
        className={cn(
          "pt-16 transition-apple min-h-screen",
          isMobile ? "pl-0" : collapsed ? "pl-[72px]" : "pl-64"
        )}
      >
        <div className="p-4 md:p-6 animate-fade-in">{children}</div>
      </main>
    </div>
  );
}

export function DashboardLayout(props: DashboardLayoutProps) {
  return (
    <SidebarProvider>
      <DashboardContent {...props} />
    </SidebarProvider>
  );
}
