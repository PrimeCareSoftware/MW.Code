/**
 * Referral Program Models
 * 
 * Models for the referral/affiliate program that allows users to
 * refer new customers and earn rewards.
 */

export interface Referral {
  id: string;
  referrerId: string;  // User who made the referral
  referrerName: string;
  referredEmail: string;  // Email of referred user
  referredName?: string;
  referredUserId?: string;  // ID once they sign up
  status: ReferralStatus;
  createdAt: Date;
  convertedAt?: Date;  // When they became a paying customer
  rewardStatus: RewardStatus;
  rewardValue?: number;  // In BRL
  rewardPaidAt?: Date;
  referralCode: string;  // Unique code for this referral
}

export enum ReferralStatus {
  PENDING = 'pending',           // Invitation sent, not signed up yet
  SIGNED_UP = 'signed_up',       // Signed up but not converted to paying
  CONVERTED = 'converted',       // Became a paying customer
  CANCELLED = 'cancelled',       // Cancelled their subscription
  EXPIRED = 'expired'            // Referral link expired (e.g., 30 days)
}

export enum RewardStatus {
  PENDING = 'pending',           // Reward not yet earned
  EARNED = 'earned',             // Reward earned but not paid
  PAID = 'paid',                 // Reward has been paid
  CANCELLED = 'cancelled'        // Reward cancelled (e.g., refund)
}

export interface ReferralStats {
  totalReferrals: number;
  pendingReferrals: number;
  signedUpReferrals: number;
  convertedReferrals: number;
  totalEarned: number;  // Total rewards earned (in BRL)
  totalPaid: number;    // Total rewards paid out
  pendingRewards: number;  // Rewards earned but not yet paid
  conversionRate: number;  // Percentage
}

export interface ReferralProgram {
  isActive: boolean;
  referralCode: string;  // User's unique referral code
  referralLink: string;  // Full URL with referral code
  rewardPerConversion: number;  // In BRL
  minimumPayoutThreshold: number;  // Minimum amount to request payout
  termsUrl: string;
  expirationDays: number;  // Days until referral link expires
}

export interface ReferralReward {
  id: string;
  userId: string;
  referralId: string;
  amount: number;  // In BRL
  status: RewardStatus;
  earnedAt: Date;
  paidAt?: Date;
  paymentMethod?: PaymentMethod;
  paymentDetails?: string;
}

export enum PaymentMethod {
  PIX = 'pix',
  BANK_TRANSFER = 'bank_transfer',
  CREDIT_TO_ACCOUNT = 'credit_to_account',  // Credit to user's subscription
  DISCOUNT_COUPON = 'discount_coupon'
}

export interface ReferralInvitation {
  email: string;
  name?: string;
  message?: string;
  referralCode: string;
}
