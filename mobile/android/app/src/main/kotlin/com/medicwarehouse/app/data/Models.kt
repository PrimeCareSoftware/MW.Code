package com.medicwarehouse.app.data

import com.google.gson.annotations.SerializedName

// Authentication Models
data class LoginRequest(
    val username: String,
    val password: String,
    val tenantId: String
)

data class LoginResponse(
    val token: String,
    val username: String,
    val role: String,
    val tenantId: String,
    val clinicId: String?,
    val expiresAt: String
)

// Patient Models
data class Patient(
    val id: String,
    val fullName: String,
    val cpf: String?,
    val dateOfBirth: String?,
    val phone: String?,
    val email: String?,
    val address: String?,
    val medicalHistory: String?,
    val allergies: String?,
    val createdAt: String
)

data class PatientListResponse(
    val items: List<Patient>,
    val totalCount: Int,
    val pageNumber: Int,
    val pageSize: Int
)

// Appointment Models
data class Appointment(
    val id: String,
    val patientId: String,
    val patientName: String?,
    val doctorId: String?,
    val doctorName: String?,
    val appointmentDate: String,
    val appointmentTime: String,
    val duration: Int,
    val status: String,
    val appointmentType: String?,
    val notes: String?
)

data class AppointmentListResponse(
    val items: List<Appointment>,
    val totalCount: Int,
    val pageNumber: Int,
    val pageSize: Int
)

// Dashboard Models
data class DashboardStats(
    val todayAppointments: Int,
    val totalPatients: Int,
    val pendingAppointments: Int,
    val completedToday: Int
)

// API Error Models
data class ApiError(
    val message: String,
    val code: String?
)
