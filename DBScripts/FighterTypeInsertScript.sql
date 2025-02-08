SET IDENTITY_INSERT FighterType ON;

INSERT INTO FighterType (Id, TypeName, TypeDesc)
VALUES
    (1, 'Human', 'Represents a human fighter'),
    (2, 'Monster', 'Represents a monster fighter');

SET IDENTITY_INSERT FighterType OFF;
