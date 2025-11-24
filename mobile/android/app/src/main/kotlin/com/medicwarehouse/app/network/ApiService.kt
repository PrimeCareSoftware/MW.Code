package com.medicwarehouse.app.network

import com.medicwarehouse.app.data.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {
    
    // Authentication
    @POST("auth/login")
    suspend fun login(@Body request: LoginRequest): Response<LoginResponse>
    
    @POST("auth/owner-login")
    suspend fun ownerLogin(@Body request: LoginRequest): Response<LoginResponse>
    
    // Patients
    @GET("patients")
    suspend fun getPatients(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("searchTerm") searchTerm: String? = null
    ): Response<PatientListResponse>
    
    @GET("patients/{id}")
    suspend fun getPatient(@Path("id") id: String): Response<Patient>
    
    @GET("patients/search")
    suspend fun searchPatients(@Query("searchTerm") searchTerm: String): Response<List<Patient>>
    
    // Appointments
    @GET("appointments")
    suspend fun getAppointments(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
        @Query("status") status: String? = null
    ): Response<AppointmentListResponse>
    
    @GET("appointments/{id}")
    suspend fun getAppointment(@Path("id") id: String): Response<Appointment>
    
    @GET("appointments/agenda")
    suspend fun getTodayAppointments(): Response<List<Appointment>>
}
