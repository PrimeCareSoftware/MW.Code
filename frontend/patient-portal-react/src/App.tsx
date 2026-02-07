import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Index from "./pages/Index";
import Dashboard from "./pages/Dashboard";
import Patients from "./pages/Patients";
import PatientForm from "./pages/PatientForm";
import PatientPortal from "./pages/PatientPortal";
import AdminDashboard from "./pages/AdminDashboard";
import Appointments from "./pages/Appointments";
import Procedures from "./pages/Procedures";
import Records from "./pages/Records";
import Inventory from "./pages/Inventory";
import Reports from "./pages/Reports";
import Settings from "./pages/Settings";
import MyAppointments from "./pages/MyAppointments";
import MyExams from "./pages/MyExams";
import MyRecords from "./pages/MyRecords";
import Notifications from "./pages/Notifications";
import Clinics from "./pages/Clinics";
import Users from "./pages/Users";
import Permissions from "./pages/Permissions";
import System from "./pages/System";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/patients" element={<Patients />} />
          <Route path="/patients/new" element={<PatientForm />} />
          <Route path="/patient-portal" element={<PatientPortal />} />
          <Route path="/admin" element={<AdminDashboard />} />
          <Route path="/appointments" element={<Appointments />} />
          <Route path="/procedures" element={<Procedures />} />
          <Route path="/records" element={<Records />} />
          <Route path="/inventory" element={<Inventory />} />
          <Route path="/reports" element={<Reports />} />
          <Route path="/settings" element={<Settings />} />
          <Route path="/my-appointments" element={<MyAppointments />} />
          <Route path="/my-exams" element={<MyExams />} />
          <Route path="/my-records" element={<MyRecords />} />
          <Route path="/notifications" element={<Notifications />} />
          <Route path="/clinics" element={<Clinics />} />
          <Route path="/users" element={<Users />} />
          <Route path="/permissions" element={<Permissions />} />
          <Route path="/system" element={<System />} />
          {/* ADD ALL CUSTOM ROUTES ABOVE THE CATCH-ALL "*" ROUTE */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
