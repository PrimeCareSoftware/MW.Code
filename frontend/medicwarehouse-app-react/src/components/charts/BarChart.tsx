import {
  BarChart as RechartsBarChart,
  Bar,
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

interface BarChartProps {
  data: DataPoint[];
  height?: number;
  showGrid?: boolean;
  barColor?: string;
  secondaryBarColor?: string;
}

export function BarChart({
  data,
  height = 300,
  showGrid = true,
  barColor = "hsl(211, 84%, 55%)",
  secondaryBarColor = "hsl(174, 62%, 47%)",
}: BarChartProps) {
  const hasSecondary = data.some((d) => d.value2 !== undefined);

  return (
    <ResponsiveContainer width="100%" height={height}>
      <RechartsBarChart data={data} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
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
          cursor={{ fill: "hsl(220, 14%, 96%)" }}
        />
        <Bar dataKey="value" fill={barColor} radius={[4, 4, 0, 0]} />
        {hasSecondary && (
          <Bar dataKey="value2" fill={secondaryBarColor} radius={[4, 4, 0, 0]} />
        )}
      </RechartsBarChart>
    </ResponsiveContainer>
  );
}
