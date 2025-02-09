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
    ElementTypeId   INT FOREIGN KEY REFERENCES ElementType(Id),
    FighterTypeId   INT FOREIGN KEY REFERENCES FighterType(Id),
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
    ElementTypeId   INT FOREIGN KEY REFERENCES ElementType(Id),    
    Name            NVARCHAR(256),
    AttackPoints    INT,
    CoolDown        INT
);

CREATE TABLE FighterAttack
(
    Id          INT PRIMARY KEY IDENTITY,
    FighterId   INT FOREIGN KEY REFERENCES Fighter(Id) ON DELETE CASCADE,
    AttackId    INT FOREIGN KEY REFERENCES Attack(Id) ON DELETE CASCADE
);

