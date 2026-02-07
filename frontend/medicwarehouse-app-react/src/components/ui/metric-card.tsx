import { cn } from "@/lib/utils";
import { LucideIcon, TrendingUp, TrendingDown } from "lucide-react";

interface MetricCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  trend?: {
    value: number;
    label: string;
    direction: "up" | "down";
  };
  icon?: LucideIcon;
  variant?: "primary" | "accent" | "success" | "warning" | "default";
  className?: string;
}

const variantStyles: Record<string, string> = {
  primary: "metric-primary border-primary/10",
  accent: "metric-accent border-accent/10",
  success: "metric-success border-success/10",
  warning: "metric-warning border-warning/10",
  default: "bg-card border-border",
};

const iconVariantStyles: Record<string, string> = {
  primary: "bg-primary/10 text-primary",
  accent: "bg-accent/10 text-accent",
  success: "bg-success/10 text-success",
  warning: "bg-warning/10 text-warning",
  default: "bg-secondary text-muted-foreground",
};

export function MetricCard({
  title,
  value,
  subtitle,
  trend,
  icon: Icon,
  variant = "default",
  className,
}: MetricCardProps) {
  const TrendIcon = trend?.direction === "up" ? TrendingUp : TrendingDown;
  const trendColor = trend?.direction === "up" ? "text-success" : "text-destructive";

  return (
    <div
      className={cn(
        "rounded-xl border p-6 shadow-apple-sm transition-apple card-hover",
        variantStyles[variant],
        className
      )}
    >
      <div className="flex items-start justify-between">
        <div className="space-y-1">
          <p className="text-sm font-medium text-muted-foreground">{title}</p>
          <p className="text-3xl font-semibold tracking-tight">{value}</p>
          {subtitle && (
            <p className="text-sm text-muted-foreground">{subtitle}</p>
          )}
        </div>
        {Icon && (
          <div className={cn("rounded-lg p-2.5", iconVariantStyles[variant])}>
            <Icon className="h-5 w-5" />
          </div>
        )}
      </div>
      {trend && (
        <div className={cn("mt-4 flex items-center gap-1.5 text-sm", trendColor)}>
          <TrendIcon className="h-4 w-4" />
          <span className="font-medium">{trend.value}%</span>
          <span className="text-muted-foreground">{trend.label}</span>
        </div>
      )}
    </div>
  );
}
