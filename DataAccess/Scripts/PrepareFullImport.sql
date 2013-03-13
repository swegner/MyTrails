DELETE FROM DrivingDirections
DELETE FROM TrailCharacteristicTrails
DELETE FROM TrailFeatureTrails
DELETE FROM TripReportTrails
DELETE FROM Addresses
DELETE FROM Trails
DELETE FROM GuideBooks
DELETE FROM TripReports
DELETE FROM Users

INSERT INTO Addresses (Location, Coordinate)
VALUES ('Seattle', geography::STPointFromText('POINT(-122.3320708 47.6062095)', 4326))