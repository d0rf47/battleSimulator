CREATE TABLE FighterType
(
    Id              INT PRIMARY KEY IDENTITY,
    TypeName        NVARCHAR(50) NOT NULL,
    TypeDesc        NVARCHAR(256)
);

CREATE TABLE ElementType
(
    Id              INT PRIMARY KEY IDENTITY,
    TypeName        NVARCHAR(50) NOT NULL,
    TypeDesc        NVARCHAR(256)
);

CREATE TABLE Fighter 
(
    Id              INT PRIMARY KEY IDENTITY,
    ElementType     INT FOREIGN KEY REFERENCES ElementType(Id),
    FighterType     INT FOREIGN KEY REFERENCES FighterType(Id),
    Name            NVARCHAR(256),
    AttackPoints    INT,
    DefensePoints   INT,
    HealthPoints    INT,
    Speed           INT,
    Level           INT
);

CREATE TABLE Attack
(
    Id              INT PRIMARY KEY IDENTITY,
    ElementType     INT FOREIGN KEY REFERENCES ElementType(Id),    
    Name            NVARCHAR(256),
    AttackPoints    INT,
    CoolDown        INT
);
