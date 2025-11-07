-- Create CustomerProfile table
CREATE TABLE CustomerProfile (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    ContactInfo NVARCHAR(200),
    PurchaseHistory NVARCHAR(MAX),
    SegmentationData NVARCHAR(200)
);

-- Create SalesOpportunity table
CREATE TABLE SalesOpportunity (
    OpportunityID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT NOT NULL,
    SalesStage NVARCHAR(50),
    EstimatedValue DECIMAL(18,2),
    ClosingDate DATE,
    FOREIGN KEY (CustomerID) REFERENCES CustomerProfile(CustomerID)
);

-- Create SupportTicket table
CREATE TABLE SupportTicket (
    TicketID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT NOT NULL,
    IssueDescription NVARCHAR(500),
    AssignedAgent NVARCHAR(100),
    Status NVARCHAR(20) CHECK (Status IN ('Open', 'Closed')),
    FOREIGN KEY (CustomerID) REFERENCES CustomerProfile(CustomerID)
);

-- Create Campaign table
CREATE TABLE Campaign (
    CampaignID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    StartDate DATE,
    EndDate DATE,
    Type NVARCHAR(50) CHECK (Type IN ('Email', 'SMS', 'Social Media'))
);

-- Create Report table
CREATE TABLE Report (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReportType NVARCHAR(50) CHECK (ReportType IN ('Sales', 'Support', 'Marketing')),
    GeneratedDate DATETIME DEFAULT GETDATE(),
    DataPoints NVARCHAR(MAX)
);
