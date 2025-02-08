SET IDENTITY_INSERT ElementType ON;

INSERT INTO ElementType (Id, TypeName, TypeDesc)
VALUES
    (1, 'Fire', 'Represents the Fire element'),
    (2, 'Grass', 'Represents the Grass element'),
    (3, 'Ground', 'Represents the Ground element'),
    (4, 'Electric', 'Represents the Electric element'),
    (5, 'Water', 'Represents the Water element'),
    (6, 'Ice', 'Represents the Ice element'),
    (7, 'Dragon', 'Represents the Dragon element'),
    (8, 'Dark', 'Represents the Dark element'),
    (9, 'Neutral', 'Represents the Neutral element');

SET IDENTITY_INSERT ElementType OFF;
