package com.medicwarehouse.app.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.medicwarehouse.app.data.LoginResponse
import com.medicwarehouse.app.data.Repository
import com.medicwarehouse.app.network.TokenManager
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.*
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class AuthViewModel @Inject constructor(
    private val repository: Repository,
    private val tokenManager: TokenManager
) : ViewModel() {
    
    private val _isAuthenticated = MutableStateFlow(false)
    val isAuthenticated: StateFlow<Boolean> = _isAuthenticated.asStateFlow()
    
    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading.asStateFlow()
    
    private val _errorMessage = MutableStateFlow<String?>(null)
    val errorMessage: StateFlow<String?> = _errorMessage.asStateFlow()
    
    private val _currentUser = MutableStateFlow<LoginResponse?>(null)
    val currentUser: StateFlow<LoginResponse?> = _currentUser.asStateFlow()
    
    init {
        checkAuthentication()
    }
    
    private fun checkAuthentication() {
        viewModelScope.launch {
            tokenManager.getToken().collect { token ->
                _isAuthenticated.value = !token.isNullOrEmpty()
            }
        }
    }
    
    fun login(username: String, password: String, tenantId: String, isOwner: Boolean = false) {
        viewModelScope.launch {
            _isLoading.value = true
            _errorMessage.value = null
            
            val result = if (isOwner) {
                repository.ownerLogin(username, password, tenantId)
            } else {
                repository.login(username, password, tenantId)
            }
            
            result.fold(
                onSuccess = { response ->
                    _currentUser.value = response
                    _isAuthenticated.value = true
                },
                onFailure = { error ->
                    _errorMessage.value = error.message ?: "Login failed"
                    _isAuthenticated.value = false
                }
            )
            
            _isLoading.value = false
        }
    }
    
    fun logout() {
        viewModelScope.launch {
            repository.logout()
            _currentUser.value = null
            _isAuthenticated.value = false
        }
    }
    
    fun clearError() {
        _errorMessage.value = null
    }
}
