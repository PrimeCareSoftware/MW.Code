import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { Button } from "@/components/ui/button";
import { Bell, Calendar, FileText, CheckCircle, AlertCircle, Info, Trash2 } from "lucide-react";
import { useState } from "react";

const initialNotifications = [
  {
    id: 1,
    type: "appointment",
    title: "Lembrete de Consulta",
    message: "Você tem uma consulta amanhã às 09:00 com Dr. João Santos.",
    time: "Há 2 horas",
    read: false,
  },
  {
    id: 2,
    type: "exam",
    title: "Resultado de Exame Disponível",
    message: "O resultado da sua Radiografia Panorâmica já está disponível para visualização.",
    time: "Há 5 horas",
    read: false,
  },
  {
    id: 3,
    type: "info",
    title: "Atualização de Dados",
    message: "Seu perfil foi atualizado com sucesso.",
    time: "Há 1 dia",
    read: true,
  },
  {
    id: 4,
    type: "alert",
    title: "Documentos Pendentes",
    message: "Por favor, atualize seus documentos de convênio.",
    time: "Há 2 dias",
    read: true,
  },
  {
    id: 5,
    type: "appointment",
    title: "Consulta Confirmada",
    message: "Sua consulta do dia 15/02 foi confirmada.",
    time: "Há 3 dias",
    read: true,
  },
];

const iconMap = {
  appointment: Calendar,
  exam: FileText,
  info: Info,
  alert: AlertCircle,
};

const colorMap = {
  appointment: "bg-primary/10 text-primary",
  exam: "bg-success/10 text-success",
  info: "bg-info/10 text-info",
  alert: "bg-warning/10 text-warning",
};

export default function Notifications() {
  const [notifications, setNotifications] = useState(initialNotifications);

  const unreadCount = notifications.filter((n) => !n.read).length;

  const markAsRead = (id: number) => {
    setNotifications(
      notifications.map((n) => (n.id === id ? { ...n, read: true } : n))
    );
  };

  const markAllAsRead = () => {
    setNotifications(notifications.map((n) => ({ ...n, read: true })));
  };

  const deleteNotification = (id: number) => {
    setNotifications(notifications.filter((n) => n.id !== id));
  };

  return (
    <DashboardLayout
      breadcrumbs={[
        { label: "Início", href: "/" },
        { label: "Notificações" },
      ]}
      variant="patient-portal"
    >
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-foreground">Notificações</h1>
            <p className="text-muted-foreground mt-1">
              {unreadCount > 0
                ? `Você tem ${unreadCount} notificação(ões) não lida(s)`
                : "Todas as notificações foram lidas"}
            </p>
          </div>
          {unreadCount > 0 && (
            <Button variant="outline" onClick={markAllAsRead} className="gap-2">
              <CheckCircle className="h-4 w-4" />
              Marcar todas como lidas
            </Button>
          )}
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center">
                <Bell className="h-5 w-5 text-primary" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total</p>
                <p className="text-xl font-semibold text-foreground">{notifications.length}</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-warning/10 flex items-center justify-center">
                <AlertCircle className="h-5 w-5 text-warning" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Não Lidas</p>
                <p className="text-xl font-semibold text-foreground">{unreadCount}</p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-success/10 flex items-center justify-center">
                <Calendar className="h-5 w-5 text-success" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Consultas</p>
                <p className="text-xl font-semibold text-foreground">
                  {notifications.filter((n) => n.type === "appointment").length}
                </p>
              </div>
            </div>
          </div>
          <div className="rounded-xl border border-border bg-card p-4 shadow-apple-sm">
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-info/10 flex items-center justify-center">
                <FileText className="h-5 w-5 text-info" />
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Exames</p>
                <p className="text-xl font-semibold text-foreground">
                  {notifications.filter((n) => n.type === "exam").length}
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Notifications List */}
        <div className="rounded-xl border border-border bg-card shadow-apple-sm overflow-hidden">
          {notifications.length === 0 ? (
            <div className="p-8 text-center">
              <Bell className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
              <p className="text-muted-foreground">Nenhuma notificação</p>
            </div>
          ) : (
            <div className="divide-y divide-border">
              {notifications.map((notification) => {
                const Icon = iconMap[notification.type as keyof typeof iconMap];
                const colorClass = colorMap[notification.type as keyof typeof colorMap];

                return (
                  <div
                    key={notification.id}
                    className={`flex items-start gap-4 p-4 hover:bg-secondary/30 transition-apple ${
                      !notification.read ? "bg-primary/5" : ""
                    }`}
                  >
                    <div className={`h-10 w-10 rounded-full flex items-center justify-center shrink-0 ${colorClass}`}>
                      <Icon className="h-5 w-5" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-start justify-between gap-4">
                        <div>
                          <p className={`font-medium ${!notification.read ? "text-foreground" : "text-muted-foreground"}`}>
                            {notification.title}
                          </p>
                          <p className="text-sm text-muted-foreground mt-1">{notification.message}</p>
                          <p className="text-xs text-muted-foreground mt-2">{notification.time}</p>
                        </div>
                        <div className="flex items-center gap-2 shrink-0">
                          {!notification.read && (
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => markAsRead(notification.id)}
                              className="text-xs"
                            >
                              Marcar como lida
                            </Button>
                          )}
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 text-muted-foreground hover:text-destructive"
                            onClick={() => deleteNotification(notification.id)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
}
