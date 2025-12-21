-- Migration: Add Ticket System
-- Date: 2025-12-21
-- Description: Creates tables for the ticket/support system migration from SystemAdmin microservice to main API

-- Create Tickets table
CREATE TABLE "Tickets" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" text NOT NULL,
    "Type" integer NOT NULL,
    "Status" integer NOT NULL,
    "Priority" integer NOT NULL,
    "UserId" uuid NOT NULL,
    "UserName" character varying(200) NOT NULL,
    "UserEmail" character varying(250) NOT NULL,
    "ClinicId" uuid,
    "ClinicName" character varying(200),
    "AssignedToId" uuid,
    "AssignedToName" character varying(200),
    "LastStatusChangeAt" timestamp without time zone,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "TenantId" character varying(100) NOT NULL,
    CONSTRAINT "PK_Tickets" PRIMARY KEY ("Id")
);

-- Create TicketComments table
CREATE TABLE "TicketComments" (
    "Id" uuid NOT NULL,
    "TicketId" uuid NOT NULL,
    "Comment" text NOT NULL,
    "AuthorId" uuid NOT NULL,
    "AuthorName" character varying(200) NOT NULL,
    "IsInternal" boolean NOT NULL,
    "IsSystemOwner" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "TenantId" character varying(100) NOT NULL,
    CONSTRAINT "PK_TicketComments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TicketComments_Tickets_TicketId" FOREIGN KEY ("TicketId") 
        REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

-- Create TicketAttachments table
CREATE TABLE "TicketAttachments" (
    "Id" uuid NOT NULL,
    "TicketId" uuid NOT NULL,
    "FileName" character varying(255) NOT NULL,
    "FileUrl" character varying(500) NOT NULL,
    "ContentType" character varying(100) NOT NULL,
    "FileSize" bigint NOT NULL,
    "UploadedAt" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "TenantId" character varying(100) NOT NULL,
    CONSTRAINT "PK_TicketAttachments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TicketAttachments_Tickets_TicketId" FOREIGN KEY ("TicketId") 
        REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

-- Create TicketHistory table
CREATE TABLE "TicketHistory" (
    "Id" uuid NOT NULL,
    "TicketId" uuid NOT NULL,
    "OldStatus" integer NOT NULL,
    "NewStatus" integer NOT NULL,
    "ChangedById" uuid NOT NULL,
    "ChangedByName" character varying(200) NOT NULL,
    "Comment" text,
    "ChangedAt" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone,
    "TenantId" character varying(100) NOT NULL,
    CONSTRAINT "PK_TicketHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TicketHistory_Tickets_TicketId" FOREIGN KEY ("TicketId") 
        REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

-- Create indexes for Tickets table
CREATE INDEX "IX_Tickets_TenantId" ON "Tickets" ("TenantId");
CREATE INDEX "IX_Tickets_TenantId_UserId" ON "Tickets" ("TenantId", "UserId");
CREATE INDEX "IX_Tickets_TenantId_ClinicId" ON "Tickets" ("TenantId", "ClinicId");
CREATE INDEX "IX_Tickets_Status_TenantId" ON "Tickets" ("Status", "TenantId");

-- Create indexes for TicketComments table
CREATE INDEX "IX_TicketComments_TicketId" ON "TicketComments" ("TicketId");
CREATE INDEX "IX_TicketComments_TenantId" ON "TicketComments" ("TenantId");

-- Create indexes for TicketAttachments table
CREATE INDEX "IX_TicketAttachments_TicketId" ON "TicketAttachments" ("TicketId");
CREATE INDEX "IX_TicketAttachments_TenantId" ON "TicketAttachments" ("TenantId");

-- Create indexes for TicketHistory table
CREATE INDEX "IX_TicketHistory_TicketId" ON "TicketHistory" ("TicketId");
CREATE INDEX "IX_TicketHistory_TenantId" ON "TicketHistory" ("TenantId");

-- Add comments for documentation
COMMENT ON TABLE "Tickets" IS 'Support tickets created by users for technical support, bug reports, feature requests, etc.';
COMMENT ON TABLE "TicketComments" IS 'Comments and updates on support tickets';
COMMENT ON TABLE "TicketAttachments" IS 'File attachments (images) uploaded to support tickets';
COMMENT ON TABLE "TicketHistory" IS 'Audit trail of status changes for support tickets';

COMMENT ON COLUMN "Tickets"."Type" IS 'Ticket type: 0=BugReport, 1=FeatureRequest, 2=SystemAdjustment, 3=FinancialIssue, 4=TechnicalSupport, 5=UserSupport, 6=Other';
COMMENT ON COLUMN "Tickets"."Status" IS 'Ticket status: 0=Open, 1=InAnalysis, 2=InProgress, 3=Blocked, 4=Completed, 5=Cancelled';
COMMENT ON COLUMN "Tickets"."Priority" IS 'Ticket priority: 0=Low, 1=Medium, 2=High, 3=Critical';
