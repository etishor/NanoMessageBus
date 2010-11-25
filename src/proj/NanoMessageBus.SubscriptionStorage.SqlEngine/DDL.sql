CREATE TABLE dbo.Subscriptions
(
    Subscriber varchar(255) NOT NULL,
    MessageType varchar(255) NOT NULL,
    Expiration smalldatetime NULL,
    CONSTRAINT PK_Subscribers PRIMARY KEY CLUSTERED ( Subscriber, MessageType )
);
CREATE UNIQUE NONCLUSTERED INDEX IX_Subscriptions ON Subscriptions
(
    MessageType,
    Expiration,
    Subscriber
);