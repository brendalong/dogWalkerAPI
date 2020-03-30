CREATE TABLE Neighborhood (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    NeighborhoodName VARCHAR(55) NOT NULL
);

CREATE TABLE DogOwner (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    OwnerName VARCHAR(55) NOT NULL,
    OwnerAddress VARCHAR (255) NOT NULL, 
    NeighborhoodId INTEGER NOT NULL, 
    PhoneNumber VARCHAR (55) NOT NULL,
    CONSTRAINT FK_DogOwner_Neighborhood FOREIGN KEY(NeighborhoodId) REFERENCES Neighborhood(Id)
    
);
CREATE TABLE Dog (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    DogName VARCHAR(55) NOT NULL,
    DogOwnerId INTEGER NOT NULL,
    Breed VARCHAR (55),
    Notes VARCHAR (255)
    CONSTRAINT FK_Dog_DogOwner FOREIGN KEY(DogOwnerId) REFERENCES DogOwner(Id)
);


CREATE TABLE Walker (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    WalkerName VARCHAR(55) NOT NULL,
    NeighborhoodId INTEGER NOT NULL,
    CONSTRAINT FK_Walker_Neighborhood FOREIGN KEY(NeighborhoodId) REFERENCES Neighborhood(Id)
);

CREATE TABLE Walks (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    Date datetime NOT NULL, 
    WalkDuration INTEGER NOT NULL,
    WalkerId INTEGER NOT NULL, 
    DogId INTEGER NOT NULL,
    CONSTRAINT FK_Walks_Walker FOREIGN KEY(WalkerId) REFERENCES Walker(Id),
    CONSTRAINT FK_Walks_Dog FOREIGN KEY(DogId) REFERENCES Dog(Id)

);


INSERT INTO Neighborhood (NeighborhoodName) VALUES ('Germantown')
INSERT INTO Neighborhood (NeighborhoodName) VALUES ('Midtown')
INSERT INTO Neighborhood (NeighborhoodName) VALUES ('Brentwood')

INSERT INTO DogOwner (OwnerName, OwnerAddress, NeighborhoodId, PhoneNumber) VALUES ('John', '14 Dakota', 1, '615-555-0787');
INSERT INTO DogOwner (OwnerName, OwnerAddress, NeighborhoodId, PhoneNumber) VALUES ('Mo', '12 South AVE', 1, '615-555-9007');
INSERT INTO DogOwner (OwnerName, OwnerAddress, NeighborhoodId, PhoneNumber) VALUES ('Adam', 'Super St', 2, '615-555-0008');
INSERT INTO DogOwner (OwnerName, OwnerAddress, NeighborhoodId, PhoneNumber) VALUES ('Steve', '223 Hallway', 3, '615-555-0000');

INSERT INTO Dog (DogName, DogOwnerId, Breed, Notes) VALUES ('Yoda', 1, 'Chica', 'The Force is strong with this one.');
INSERT INTO Dog (DogName, DogOwnerId, Breed, Notes) VALUES ('Luke', 2, 'Dog', 'The travelor');
INSERT INTO Dog (DogName, DogOwnerId, Breed, Notes) VALUES ('Darth', 3, 'Lab', 'Must say off counter');
INSERT INTO Dog (DogName, DogOwnerId, Breed, Notes) VALUES ('Leigha', 4, 'Dobermin', 'Barker');
INSERT INTO Dog (DogName, DogOwnerId, Breed, Notes) VALUES ('Esme', 1, 'Twist', 'A little of this and a little of that');


INSERT INTO Walker (WalkerName, NeighborhoodId) VALUES ('Bell', 1);
INSERT INTO Walker (WalkerName, NeighborhoodId) VALUES ('Bob', 2);
INSERT INTO Walker (WalkerName, NeighborhoodId) VALUES ('Sue', 3);

INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('01/13/2020', 30, 1, 1);
INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('02/13/2020', 30, 1, 2);
INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('03/13/2020', 30, 2, 4);
INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('04/13/2020', 30, 2, 3);
INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('05/13/2020', 30, 3, 2);
INSERT INTO Walks ([Date], WalkDuration, WalkerId, DogId) VALUES ('06/13/2020', 30, 3, 4);
