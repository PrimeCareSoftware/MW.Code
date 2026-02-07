import {
  AreaChart as RechartsAreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

interface DataPoint {
  name: string;
  value: number;
  value2?: number;
}

interface AreaChartProps {
  data: DataPoint[];
  height?: number;
  showGrid?: boolean;
  gradientFrom?: string;
  gradientTo?: string;
  secondaryGradientFrom?: string;
  secondaryGradientTo?: string;
}

export function AreaChart({
  data,
  height = 300,
  showGrid = true,
  gradientFrom = "hsl(211, 84%, 55%)",
  gradientTo = "hsl(211, 84%, 55%)",
  secondaryGradientFrom = "hsl(174, 62%, 47%)",
  secondaryGradientTo = "hsl(174, 62%, 47%)",
}: AreaChartProps) {
  const hasSecondary = data.some((d) => d.value2 !== undefined);

  return (
    <ResponsiveContainer width="100%" height={height}>
      <RechartsAreaChart data={data} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
        <defs>
          <linearGradient id="colorPrimary" x1="0" y1="0" x2="0" y2="1">
            <stop offset="5%" stopColor={gradientFrom} stopOpacity={0.3} />
            <stop offset="95%" stopColor={gradientTo} stopOpacity={0} />
          </linearGradient>
          <linearGradient id="colorSecondary" x1="0" y1="0" x2="0" y2="1">
            <stop offset="5%" stopColor={secondaryGradientFrom} stopOpacity={0.3} />
            <stop offset="95%" stopColor={secondaryGradientTo} stopOpacity={0} />
          </linearGradient>
        </defs>
        {showGrid && <CartesianGrid strokeDasharray="3 3" stroke="hsl(220, 13%, 91%)" vertical={false} />}
        <XAxis
          dataKey="name"
          axisLine={false}
          tickLine={false}
          tick={{ fill: "hsl(220, 9%, 46%)", fontSize: 12 }}
          dy={10}
        />
        <YAxis
          axisLine={false}
          tickLine={false}
          tick={{ fill: "hsl(220, 9%, 46%)", fontSize: 12 }}
          dx={-10}
        />
        <Tooltip
          contentStyle={{
            backgroundColor: "hsl(0, 0%, 100%)",
            border: "1px solid hsl(220, 13%, 91%)",
            borderRadius: "8px",
            boxShadow: "0 4px 6px -1px rgb(0 0 0 / 0.05)",
          }}
          labelStyle={{ color: "hsl(220, 9%, 12%)", fontWeight: 500 }}
        />
        <Area
          type="monotone"
          dataKey="value"
          stroke={gradientFrom}
          strokeWidth={2}
          fillOpacity={1}
          fill="url(#colorPrimary)"
        />
        {hasSecondary && (
          <Area
            type="monotone"
            dataKey="value2"
            stroke={secondaryGradientFrom}
            strokeWidth={2}
            fillOpacity={1}
            fill="url(#colorSecondary)"
          />
        )}
      </RechartsAreaChart>
    </ResponsiveContainer>
  );
}
