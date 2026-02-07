import { cn } from "@/lib/utils";

interface ToggleSwitchProps {
  checked: boolean;
  onChange: (checked: boolean) => void;
  label?: string;
  description?: string;
  disabled?: boolean;
  size?: "sm" | "md";
}

export function ToggleSwitch({
  checked,
  onChange,
  label,
  description,
  disabled = false,
  size = "md",
}: ToggleSwitchProps) {
  const sizeStyles = {
    sm: {
      track: "h-5 w-9",
      thumb: "h-4 w-4",
      translate: checked ? "translate-x-4" : "translate-x-0.5",
    },
    md: {
      track: "h-6 w-11",
      thumb: "h-5 w-5",
      translate: checked ? "translate-x-5" : "translate-x-0.5",
    },
  };

  return (
    <label
      className={cn(
        "flex items-start gap-3 cursor-pointer",
        disabled && "cursor-not-allowed opacity-50"
      )}
    >
      <button
        type="button"
        role="switch"
        aria-checked={checked}
        disabled={disabled}
        onClick={() => !disabled && onChange(!checked)}
        className={cn(
          "relative inline-flex shrink-0 rounded-full transition-apple focus:outline-none focus:ring-2 focus:ring-primary/20 focus:ring-offset-2",
          checked ? "bg-primary" : "bg-input",
          sizeStyles[size].track
        )}
      >
        <span
          className={cn(
            "inline-block rounded-full bg-card shadow-sm transition-transform duration-200",
            sizeStyles[size].thumb,
            sizeStyles[size].translate,
            "mt-0.5"
          )}
        />
      </button>
      {(label || description) && (
        <div className="space-y-0.5">
          {label && <p className="text-sm font-medium text-foreground">{label}</p>}
          {description && (
            <p className="text-sm text-muted-foreground">{description}</p>
          )}
        </div>
      )}
    </label>
  );
}
