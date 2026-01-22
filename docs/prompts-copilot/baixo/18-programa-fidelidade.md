# 🎁 Prompt: Programa de Indicação e Fidelidade

## 📊 Status
- **Prioridade**: BAIXA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1-2 meses | 1 dev
- **Prazo**: 2027+

## 🎯 Contexto

Sistema de recompensas onde pacientes acumulam pontos por consultas, ganham descontos e bônus por indicação de amigos, com níveis de fidelidade (Bronze, Prata, Ouro, Platinum) e benefícios progressivos.

## 📋 Justificativa

### Benefícios
- ✅ Aquisição orgânica (indicações)
- ✅ Retenção de pacientes
- ✅ Aumento de LTV
- ✅ Marketing boca a boca
- ✅ Gamificação

## 🏗️ Arquitetura

```csharp
// Loyalty Program
public class LoyaltyAccount : Entity
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public LoyaltyTier Tier { get; set; }
    public int ConsultationCount { get; set; }
    public int ReferralCount { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

public enum LoyaltyTier
{
    Bronze = 0,      // 0-4 consultas
    Silver = 1,      // 5-9 consultas
    Gold = 2,        // 10-19 consultas
    Platinum = 3     // 20+ consultas
}

// Referral
public class Referral : Entity
{
    public Guid Id { get; set; }
    public Guid ReferrerId { get; set; }
    public string ReferralCode { get; set; }
    public Guid? ReferredPatientId { get; set; }
    public string ReferredEmail { get; set; }
    public ReferralStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int BonusPoints { get; set; }
}

// Points Transaction
public class PointsTransaction : Entity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType Type { get; set; }
    public int Points { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum TransactionType
{
    Earned,     // Ganhou pontos
    Redeemed,   // Resgatou pontos
    Expired     // Pontos expiraram
}

// Reward
public class Reward : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PointsCost { get; set; }
    public RewardType Type { get; set; }
    public decimal DiscountAmount { get; set; }
    public int DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
}

public enum RewardType
{
    PercentDiscount,    // Desconto percentual
    FixedDiscount,      // Desconto fixo
    FreeConsultation,   // Consulta grátis
    FreeExam            // Exame grátis
}
```

## 🎯 Regras de Pontuação

```csharp
public class LoyaltyRules
{
    public static int PointsPerConsultation = 10;
    public static int PointsPerReferral = 50;
    public static int PointsForReview = 5;
    public static int PointsExpirationDays = 365;
    
    public static Dictionary<LoyaltyTier, TierBenefits> TierBenefits = new()
    {
        {
            LoyaltyTier.Bronze,
            new TierBenefits
            {
                DiscountPercentage = 0,
                PriorityBooking = false,
                BirthdayBonus = 10
            }
        },
        {
            LoyaltyTier.Silver,
            new TierBenefits
            {
                DiscountPercentage = 5,
                PriorityBooking = false,
                BirthdayBonus = 25
            }
        },
        {
            LoyaltyTier.Gold,
            new TierBenefits
            {
                DiscountPercentage = 10,
                PriorityBooking = true,
                BirthdayBonus = 50
            }
        },
        {
            LoyaltyTier.Platinum,
            new TierBenefits
            {
                DiscountPercentage = 15,
                PriorityBooking = true,
                BirthdayBonus = 100,
                FreeAnnualCheckup = true
            }
        }
    };
}

public class TierBenefits
{
    public int DiscountPercentage { get; set; }
    public bool PriorityBooking { get; set; }
    public int BirthdayBonus { get; set; }
    public bool FreeAnnualCheckup { get; set; }
}
```

## 🎨 Frontend (Angular)

```typescript
@Component({
  selector: 'app-loyalty-dashboard',
  template: `
    <mat-card class="loyalty-card">
      <div class="tier-badge" [ngClass]="account.tier">
        <mat-icon>stars</mat-icon>
        <h2>{{ account.tier }}</h2>
      </div>
      
      <div class="points">
        <h3>{{ account.availablePoints }} pontos</h3>
        <p>{{ pointsToNextTier }} pontos para o próximo nível</p>
      </div>
      
      <mat-progress-bar mode="determinate" [value]="progressToNextTier">
      </mat-progress-bar>
    </mat-card>
    
    <mat-card class="referral-card">
      <h3>Indique e Ganhe</h3>
      <p>Compartilhe seu código e ganhe {{ bonusPoints }} pontos por indicação!</p>
      
      <div class="referral-code">
        <input readonly [value]="referralCode">
        <button mat-icon-button (click)="copyReferralCode()">
          <mat-icon>content_copy</mat-icon>
        </button>
      </div>
      
      <div class="share-buttons">
        <button mat-raised-button (click)="shareWhatsApp()">
          <mat-icon>whatsapp</mat-icon>
          WhatsApp
        </button>
        <button mat-raised-button (click)="shareEmail()">
          <mat-icon>email</mat-icon>
          Email
        </button>
      </div>
    </mat-card>
    
    <mat-card class="rewards-card">
      <h3>Resgatar Pontos</h3>
      
      <div class="rewards-grid">
        <mat-card *ngFor="let reward of rewards" class="reward-item">
          <h4>{{ reward.name }}</h4>
          <p>{{ reward.description }}</p>
          <div class="reward-cost">
            <mat-icon>stars</mat-icon>
            {{ reward.pointsCost }} pontos
          </div>
          <button mat-raised-button color="primary"
                  [disabled]="account.availablePoints < reward.pointsCost"
                  (click)="redeemReward(reward)">
            Resgatar
          </button>
        </mat-card>
      </div>
    </mat-card>
    
    <mat-card class="transactions-card">
      <h3>Histórico de Pontos</h3>
      
      <mat-list>
        <mat-list-item *ngFor="let transaction of transactions">
          <mat-icon mat-list-icon [color]="transaction.type === 'Earned' ? 'primary' : 'warn'">
            {{ transaction.type === 'Earned' ? 'add_circle' : 'remove_circle' }}
          </mat-icon>
          <div mat-line>{{ transaction.description }}</div>
          <div mat-line>{{ transaction.createdAt | date:'short' }}</div>
          <span class="points">{{ transaction.points > 0 ? '+' : '' }}{{ transaction.points }}</span>
        </mat-list-item>
      </mat-list>
    </mat-card>
  `
})
export class LoyaltyDashboardComponent implements OnInit {
  account: LoyaltyAccount;
  referralCode: string;
  rewards: Reward[] = [];
  transactions: PointsTransaction[] = [];
  
  constructor(
    private loyaltyService: LoyaltyService,
    private snackBar: MatSnackBar
  ) {}
  
  async ngOnInit() {
    this.account = await this.loyaltyService.getAccount();
    this.referralCode = await this.loyaltyService.getReferralCode();
    this.rewards = await this.loyaltyService.getRewards();
    this.transactions = await this.loyaltyService.getTransactions();
  }
  
  get pointsToNextTier(): number {
    const nextTier = this.getNextTier();
    return nextTier ? nextTier.requiredPoints - this.account.totalPoints : 0;
  }
  
  get progressToNextTier(): number {
    const nextTier = this.getNextTier();
    if (!nextTier) return 100;
    
    const currentTier = this.getCurrentTier();
    const progress = (this.account.totalPoints - currentTier.requiredPoints) /
                    (nextTier.requiredPoints - currentTier.requiredPoints);
    
    return Math.min(progress * 100, 100);
  }
  
  copyReferralCode() {
    navigator.clipboard.writeText(this.referralCode);
    this.snackBar.open('Código copiado!', 'OK', { duration: 2000 });
  }
  
  shareWhatsApp() {
    const message = `Use meu código ${this.referralCode} e ganhe desconto na primeira consulta!`;
    const url = `https://wa.me/?text=${encodeURIComponent(message)}`;
    window.open(url, '_blank');
  }
  
  async redeemReward(reward: Reward) {
    const confirmed = confirm(`Resgatar ${reward.name} por ${reward.pointsCost} pontos?`);
    if (confirmed) {
      await this.loyaltyService.redeemReward(reward.id);
      this.snackBar.open('Recompensa resgatada!', 'OK', { duration: 3000 });
      await this.ngOnInit();
    }
  }
}
```

### Service

```typescript
@Injectable({ providedIn: 'root' })
export class LoyaltyService {
  private apiUrl = '/api/loyalty';
  
  constructor(private http: HttpClient) {}
  
  getAccount(): Promise<LoyaltyAccount> {
    return firstValueFrom(
      this.http.get<LoyaltyAccount>(`${this.apiUrl}/account`)
    );
  }
  
  getReferralCode(): Promise<string> {
    return firstValueFrom(
      this.http.get<{ code: string }>(`${this.apiUrl}/referral-code`)
        .pipe(map(r => r.code))
    );
  }
  
  getRewards(): Promise<Reward[]> {
    return firstValueFrom(
      this.http.get<Reward[]>(`${this.apiUrl}/rewards`)
    );
  }
  
  redeemReward(rewardId: string): Promise<void> {
    return firstValueFrom(
      this.http.post<void>(`${this.apiUrl}/redeem`, { rewardId })
    );
  }
  
  getTransactions(): Promise<PointsTransaction[]> {
    return firstValueFrom(
      this.http.get<PointsTransaction[]>(`${this.apiUrl}/transactions`)
    );
  }
}
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar entidades (LoyaltyAccount, Referral, PointsTransaction, Reward)
- [ ] Implementar repositórios
- [ ] Criar LoyaltyService
- [ ] Sistema de pontuação
- [ ] Cálculo de níveis
- [ ] Geração de códigos de indicação
- [ ] Expiração de pontos
- [ ] Controllers REST
- [ ] Migrations

### Frontend
- [ ] LoyaltyDashboardComponent
- [ ] ReferralShareComponent
- [ ] RewardsListComponent
- [ ] TransactionsHistoryComponent
- [ ] TierBadgeComponent
- [ ] LoyaltyService (Angular)

### Integrações
- [ ] Notificações de pontos ganhos
- [ ] Email de boas-vindas ao programa
- [ ] Notificação de nível alcançado
- [ ] Lembrete de pontos expirando

### Testes
- [ ] Testes unitários
- [ ] Testes de regras de pontuação
- [ ] Testes de resgate
- [ ] Testes de indicação

## 💰 Investimento

- **Esforço**: 1-2 meses | 1 dev
- **Custo**: R$ 45-90k

### ROI Esperado
- Aquisição orgânica: +20-30%
- Retenção: +15-25%
- LTV aumentado: +30-40%
- CAC reduzido: -25%

## 🎯 Critérios de Aceitação

- [ ] Sistema de pontos funciona
- [ ] Níveis são calculados automaticamente
- [ ] Indicações geram bônus
- [ ] Resgate de recompensas funciona
- [ ] Pontos expiram após 1 ano
- [ ] Dashboard visual e intuitivo
- [ ] Compartilhamento social funciona
- [ ] Notificações de pontos ganhos
- [ ] Histórico de transações visível
- [ ] Benefícios por nível aplicados
