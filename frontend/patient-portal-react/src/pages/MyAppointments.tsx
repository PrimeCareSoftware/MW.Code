import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { StatusBadge } from "@/components/ui/status-badge";
import { Plus, Calendar, Clock, MapPin, User } from "lucide-react";

const appointments = [
  {
    id: 1,
    doctor: "Dr. João Santos",
    specialty: "Clínico Geral",
    date: "10/02/2026",
    time: "09:00",
    location: "Sala 101",
    status: "confirmed",
  },
  {
    id: 2,
    doctor: "Dra. Ana Lima",
    specialty: "Ortodontia",
    date: "15/02/2026",
    time: "14:30",
    location: "Sala 203",
    status: "pending",
  },
  {
    id: 3,
    doctor: "Dr. Carlos Mendes",
    specialty: "Endodontia",
    date: "22/02/2026",
    time: "10:00",
    location: "Sala 105",
    status: "confirmed",
  },
];

const pastAppointments = [
  {
    id: 4,
    doctor: "Dr. João Santos",
    specialty: "Clínico Geral",
    date: "05/02/2026",
    time: "11:00",
    location: "Sala 101",
    status: "completed",
  },
  {
    id: 5,
    doctor: "Dra. Ana Lima",
    specialty: "Ortodontia",
    date: "20/01/2026",
    time: "15:00",
    location: "Sala 203",
    status: "completed",
  },
];

export default function MyAppointments() {
  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Início", href: "/" },
        { label: "Minhas Consultas" },
      ]}
      variant="patient-portal"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Minhas Consultas</h1>
            <p className="text-muted-foreground mt-1">
              Acompanhe e gerencie suas consultas agendadas
            </p>
          </div>
          <Button className="gap-2">
            <Plus className="h-4 w-4" />
            Agendar Consulta
          </Button>
        </div>

        {/* Upcoming Appointments */}
        <div>
          <h2 className="text-lg font-semibold text-foreground mb-4">Próximas Consultas</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {appointments.map((apt) => (
              <div
                key={apt.id}
                className="rounded-xl border border-border bg-card p-5 shadow-apple-sm hover:shadow-apple-md transition-apple"
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="h-12 w-12 rounded-full bg-primary/10 flex items-center justify-center">
                      <User className="h-6 w-6 text-primary" />
                    </div>
                    <div>
                      <p className="font-semibold text-foreground">{apt.doctor}</p>
                      <p className="text-sm text-muted-foreground">{apt.specialty}</p>
                    </div>
                  </div>
                  <StatusBadge
                    status={apt.status as "confirmed" | "pending"}
                    labels={{
                      confirmed: "Confirmado",
                      pending: "Pendente",
                    }}
                  />
                </div>
                <div className="space-y-2">
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{apt.date}</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Clock className="h-4 w-4" />
                    <span>{apt.time}</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <MapPin className="h-4 w-4" />
                    <span>{apt.location}</span>
                  </div>
                </div>
                <div className="flex gap-2 mt-4 pt-4 border-t border-border">
                  <Button variant="outline" size="sm" className="flex-1">
                    Reagendar
                  </Button>
                  <Button variant="outline" size="sm" className="flex-1 text-destructive hover:text-destructive">
                    Cancelar
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Past Appointments */}
        <div>
          <h2 className="text-lg font-semibold text-foreground mb-4">Histórico de Consultas</h2>
          <div className="rounded-xl border border-border bg-card shadow-apple-sm overflow-hidden">
            {pastAppointments.map((apt, index) => (
              <div
                key={apt.id}
                className={`flex items-center justify-between p-4 ${
                  index !== pastAppointments.length - 1 ? "border-b border-border" : ""
                }`}
              >
                <div className="flex items-center gap-4">
                  <div className="h-10 w-10 rounded-full bg-secondary flex items-center justify-center">
                    <User className="h-5 w-5 text-muted-foreground" />
                  </div>
                  <div>
                    <p className="font-medium text-foreground">{apt.doctor}</p>
                    <p className="text-sm text-muted-foreground">{apt.specialty}</p>
                  </div>
                </div>
                <div className="flex items-center gap-6">
                  <div className="text-right">
                    <p className="text-sm font-medium text-foreground">{apt.date}</p>
                    <p className="text-sm text-muted-foreground">{apt.time}</p>
                  </div>
                  <StatusBadge
                    status="completed"
                    labels={{ completed: "Realizada" }}
                  />
                  <Button variant="ghost" size="sm">
                    Ver Detalhes
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
