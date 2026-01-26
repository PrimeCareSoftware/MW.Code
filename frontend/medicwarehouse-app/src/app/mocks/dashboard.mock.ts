/**
 * Mock data for Dashboard statistics
 * Used only when environment.useMockData is true
 */

export interface DashboardQueueData {
  waitingQueue: number;
  patientsGrowth: number;
}

export const MOCK_DASHBOARD_QUEUE_DATA: DashboardQueueData = {
  waitingQueue: 2,
  patientsGrowth: 12
};
