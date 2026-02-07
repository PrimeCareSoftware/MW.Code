import { cn } from "@/lib/utils";

type BadgeVariant = "default" | "success" | "warning" | "destructive" | "info" | "outline";

type StatusBadgePropsWithVariant = {
  children: React.ReactNode;
  variant?: BadgeVariant;
  size?: "sm" | "md";
  dot?: boolean;
  className?: string;
  status?: never;
  labels?: never;
};

type StatusBadgePropsWithStatus = {
  status: string;
  labels: Record<string, string>;
  variant?: never;
  children?: never;
  size?: "sm" | "md";
  dot?: boolean;
  className?: string;
};

type StatusBadgeProps = StatusBadgePropsWithVariant | StatusBadgePropsWithStatus;

const variantStyles: Record<BadgeVariant, string> = {
  default: "bg-secondary text-secondary-foreground",
  success: "bg-success/10 text-success",
  warning: "bg-warning/10 text-warning",
  destructive: "bg-destructive/10 text-destructive",
  info: "bg-primary/10 text-primary",
  outline: "border border-border bg-transparent text-muted-foreground",
};

const dotStyles: Record<BadgeVariant, string> = {
  default: "bg-secondary-foreground",
  success: "bg-success",
  warning: "bg-warning",
  destructive: "bg-destructive",
  info: "bg-primary",
  outline: "bg-muted-foreground",
};

const statusToVariant: Record<string, BadgeVariant> = {
  active: "success",
  online: "success",
  confirmed: "success",
  completed: "success",
  ready: "success",
  ok: "success",
  running: "success",
  pending: "warning",
  low: "warning",
  warning: "warning",
  maintenance: "warning",
  inactive: "default",
  offline: "default",
  cancelled: "destructive",
  critical: "destructive",
  error: "destructive",
};

export function StatusBadge(props: StatusBadgeProps) {
  const size = props.size ?? "md";
  const dot = props.dot ?? false;
  const className = props.className;

  let variant: BadgeVariant;
  let displayText: React.ReactNode;

  if ("status" in props && props.status !== undefined) {
    variant = statusToVariant[props.status] || "default";
    displayText = props.labels[props.status] || props.status;
  } else {
    variant = props.variant ?? "default";
    displayText = props.children;
  }

  return (
    <span
      className={cn(
        "inline-flex items-center gap-1.5 rounded-full font-medium",
        size === "sm" ? "px-2 py-0.5 text-xs" : "px-2.5 py-1 text-xs",
        variantStyles[variant],
        className
      )}
    >
      {dot && <span className={cn("h-1.5 w-1.5 rounded-full", dotStyles[variant])} />}
      {displayText}
    </span>
  );
}
