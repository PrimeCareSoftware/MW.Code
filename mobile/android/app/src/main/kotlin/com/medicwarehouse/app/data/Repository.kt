package com.medicwarehouse.app.data

import com.medicwarehouse.app.network.ApiService
import com.medicwarehouse.app.network.TokenManager
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class Repository @Inject constructor(
    private val apiService: ApiService,
    private val tokenManager: TokenManager
) {
    // Authentication
    suspend fun login(username: String, password: String, tenantId: String): Result<LoginResponse> {
        return try {
            val response = apiService.login(LoginRequest(username, password, tenantId))
            if (response.isSuccessful && response.body() != null) {
                val loginResponse = response.body()!!
                tokenManager.saveToken(loginResponse.token)
                tokenManager.saveUserInfo(loginResponse.username, loginResponse.role, loginResponse.tenantId)
                Result.success(loginResponse)
            } else {
                Result.failure(Exception(response.message() ?: "Login failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun ownerLogin(username: String, password: String, tenantId: String): Result<LoginResponse> {
        return try {
            val response = apiService.ownerLogin(LoginRequest(username, password, tenantId))
            if (response.isSuccessful && response.body() != null) {
                val loginResponse = response.body()!!
                tokenManager.saveToken(loginResponse.token)
                tokenManager.saveUserInfo(loginResponse.username, loginResponse.role, loginResponse.tenantId)
                Result.success(loginResponse)
            } else {
                Result.failure(Exception(response.message() ?: "Login failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun logout() {
        tokenManager.clearToken()
    }
    
    // Patients
    suspend fun getPatients(page: Int = 1, pageSize: Int = 20, searchTerm: String? = null): Result<PatientListResponse> {
        return try {
            val response = apiService.getPatients(page, pageSize, searchTerm)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.message() ?: "Failed to get patients"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getPatient(id: String): Result<Patient> {
        return try {
            val response = apiService.getPatient(id)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.message() ?: "Failed to get patient"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Appointments
    suspend fun getAppointments(page: Int = 1, pageSize: Int = 20, status: String? = null): Result<AppointmentListResponse> {
        return try {
            val response = apiService.getAppointments(page, pageSize, status)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.message() ?: "Failed to get appointments"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getTodayAppointments(): Result<List<Appointment>> {
        return try {
            val response = apiService.getTodayAppointments()
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.message() ?: "Failed to get appointments"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Dashboard Stats
    suspend fun getDashboardStats(): Result<DashboardStats> {
        return try {
            val todayAppointmentsResult = getTodayAppointments()
            val patientsResult = getPatients(1, 1)
            
            if (todayAppointmentsResult.isSuccess && patientsResult.isSuccess) {
                val appointments = todayAppointmentsResult.getOrNull() ?: emptyList()
                val totalPatients = patientsResult.getOrNull()?.totalCount ?: 0
                
                val completed = appointments.count { it.status.lowercase() == "completed" }
                val pending = appointments.count { it.status.lowercase() == "scheduled" }
                
                Result.success(
                    DashboardStats(
                        todayAppointments = appointments.size,
                        totalPatients = totalPatients,
                        pendingAppointments = pending,
                        completedToday = completed
                    )
                )
            } else {
                Result.failure(Exception("Failed to get dashboard stats"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
