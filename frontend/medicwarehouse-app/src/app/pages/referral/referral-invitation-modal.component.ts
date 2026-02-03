import { Component, Inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { Subject, takeUntil, Observable } from 'rxjs';
import { ReferralService, ReferralInvitation } from '../../services/referral/referral.service';

@Component({
  selector: 'app-referral-invitation-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './referral-invitation-modal.component.html',
  styleUrls: ['./referral-invitation-modal.component.scss']
})
export class ReferralInvitationModalComponent implements OnDestroy {
  invitationForm: FormGroup;
  submitting = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ReferralInvitationModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { referralCode: string; referralLink: string },
    private referralService: ReferralService
  ) {
    this.invitationForm = this.fb.group({
      emails: this.fb.array([
        this.createEmailControl()
      ]),
      personalMessage: ['']
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get emails(): FormArray {
    return this.invitationForm.get('emails') as FormArray;
  }

  private createEmailControl(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  addEmail(): void {
    if (this.emails.length < 10) {
      this.emails.push(this.createEmailControl());
    }
  }

  removeEmail(index: number): void {
    if (this.emails.length > 1) {
      this.emails.removeAt(index);
    }
  }

  onSubmit(): void {
    if (this.invitationForm.invalid) {
      this.invitationForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    const formValue = this.invitationForm.value;
    const emailList = formValue.emails.map((e: any) => e.email).filter((email: string) => email);

    // Send invitations
    const invitations = emailList.map((email: string) => {
      const invitation: ReferralInvitation = {
        email,
        message: formValue.personalMessage,
        referralCode: this.data.referralCode
      };
      return this.referralService.sendReferralInvitation(invitation);
    });

    // Wait for all invitations to be sent
    Promise.all(invitations.map((obs: Observable<any>) => obs.toPromise()))
      .then(() => {
        this.submitting = false;
        this.dialogRef.close(true);
      })
      .catch(error => {
        console.error('Error sending invitations:', error);
        this.submitting = false;
      });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getEmailPreview(): string {
    const personalMessage = this.invitationForm.get('personalMessage')?.value || '';
    return `
Olá! 👋

${personalMessage || 'Gostaria de te convidar para conhecer o Omni Care, o melhor software de gestão clínica do mercado!'}

Com o Omni Care você consegue:
✅ Gerenciar consultas e pacientes
✅ Prontuário eletrônico completo
✅ Controle financeiro integrado
✅ Lembretes automáticos via WhatsApp

E o melhor: ganhe 15 dias grátis usando meu link de indicação!

👉 ${this.data.referralLink}

Código de indicação: ${this.data.referralCode}

Experimente agora e veja como é fácil gerenciar sua clínica!

Abraço!
    `.trim();
  }
}
