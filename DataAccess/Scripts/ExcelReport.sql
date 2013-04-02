SELECT
	tra.Name,
	tra.Url,
	tra.Location,
	tra.WtaRating Rating,
	tra.Mileage,
	tra.ElevationGain,
	tra.HighPoint,
	r.Name Region,
	rp.Name RequiredPass,
	g.Title Guidebook,
	dSeattle.DrivingTimeSeconds SeattleDrivingTimeSeconds,
	ntr.NumTrailReports,
	ltr.LatestTripReportDate,
	ISNULL(c.GoodForKids, 0) GoodForKids,
	ISNULL(c.DogsAllowedOnLeash, 0) DogsAllowedOnLeash,
	ISNULL(c.DogsAllowedOffLeash, 0) DogsAllowedOffLeash,
	ISNULL(c.DogsNotAllowed, 0) DogsNotAllowed,
	ISNULL(c.MayEncounterPackAnimals, 0) MayEncounterPackAnimals,
	ISNULL(c.MayEncounterMountainBikes, 0) MayEncounterMountainBikes,
	ISNULL(c.MayEncounterMotorizedVehicles,	0) MayEncounterMotorizedVehicles,
	ISNULL(c.PermitOrPassRequired, 0) PermitOrPassRequired,
	ISNULL(f.Coast, 0) Coast,
	ISNULL(f.Rivers, 0) Rivers,
	ISNULL(f.Lakes, 0) Lakes,
	ISNULL(f.Waterfalls, 0) Waterfalls,
	ISNULL(f.OldGrowth, 0) OldGrowth,
	ISNULL(f.FallFoliage, 0) FallFoliage,
	ISNULL(f.WildflowersMeadows, 0) WildflowersMeadows,
	ISNULL(f.MountainViews, 0) MountainViews,
	ISNULL(f.Summits, 0) Summits,
	ISNULL(f.Wildlife, 0) Wildlife,
	ISNULL(f.RidgesPasses, 0) RidgesPasses,
	ISNULL(f.EstablishedCampsites, 0) EstablishedCampsites
FROM Trails tra WITH (NOLOCK)
	LEFT OUTER JOIN Regions r WITH (NOLOCK) ON tra.RegionId = r.Id
	LEFT OUTER JOIN RequiredPasses rp WITH (NOLOCK) ON tra.RequiredPassId = rp.Id
	LEFT OUTER JOIN Guidebooks g WITH (NOLOCK) ON tra.GuidebookId = g.Id
	LEFT OUTER JOIN
	(
		SELECT 
			trt.Trail_Id,
			COUNT(*) AS NumTrailReports
		FROM TripReports tr WITH (NOLOCK)
			JOIN TripReportTrails trt WITH (NOLOCK) ON tr.Id = trt.TripReport_Id
		GROUP BY trt.Trail_Id
	) ntr ON ntr.Trail_Id = tra.Id
	LEFT OUTER JOIN
	(
		SELECT
			r.Trail_Id,
			r.Date LatestTripReportDate
		FROM
		(
			SELECT
				ROW_NUMBER() OVER (PARTITION BY trt.Trail_Id ORDER BY tr.Date DESC) RowNumber,
				trt.Trail_Id,
				tr.Date
			FROM TripReportTrails trt WITH (NOLOCK)
				JOIN TripReports tr WITH (NOLOCK) ON trt.TripReport_Id = tr.Id
		) r
		WHERE r.RowNumber = 1
	) ltr ON ltr.Trail_Id = tra.Id
	LEFT OUTER JOIN 
	(
		SELECT 
			dd.TrailId,
			dd.DrivingTimeSeconds
		FROM DrivingDirections dd WITH (NOLOCK)
			JOIN Addresses a WITH (NOLOCK) ON dd.AddressId = a.Id
		WHERE a.Location = 'Seattle'
	) dSeattle ON dSeattle.TrailId = tra.Id
	LEFT OUTER JOIN
	(
		SELECT
			tft.Trail_Id,
			SUM(CASE WHEN tf.Description = 'Coast' THEN 1 ELSE 0 END) AS Coast,
			SUM(CASE WHEN tf.Description = 'Rivers' THEN 1 ELSE 0 END) AS Rivers,
			SUM(CASE WHEN tf.Description = 'Lakes' THEN 1 ELSE 0 END) AS Lakes,
			SUM(CASE WHEN tf.Description = 'Waterfalls' THEN 1 ELSE 0 END) AS Waterfalls,
			SUM(CASE WHEN tf.Description = 'Old Growth' THEN 1 ELSE 0 END) AS OldGrowth,
			SUM(CASE WHEN tf.Description = 'Fall Foliage' THEN 1 ELSE 0 END) AS FallFoliage,
			SUM(CASE WHEN tf.Description = 'Wildflowers / Meadows' THEN 1 ELSE 0 END) AS WildflowersMeadows,
			SUM(CASE WHEN tf.Description = 'Mountain Views' THEN 1 ELSE 0 END) AS MountainViews,
			SUM(CASE WHEN tf.Description = 'Summits' THEN 1 ELSE 0 END) AS Summits,
			SUM(CASE WHEN tf.Description = 'Wildlife' THEN 1 ELSE 0 END) AS Wildlife,
			SUM(CASE WHEN tf.Description = 'Ridges / Passes' THEN 1 ELSE 0 END) AS RidgesPasses,
			SUM(CASE WHEN tf.Description = 'Established Campsites' THEN 1 ELSE 0 END) AS EstablishedCampsites
		FROM TrailFeatureTrails tft WITH (NOLOCK)
			JOIN TrailFeatures tf WITH (NOLOCK) ON tft.TrailFeature_Id = tf.Id
		GROUP BY tft.Trail_Id
	) f ON f.Trail_Id = tra.Id
	LEFT OUTER JOIN
	(
		SELECT
			tct.Trail_Id,
			SUM(CASE WHEN tc.Description = 'Good for Kids' THEN 1 ELSE 0 END) AS GoodForKids,
			SUM(CASE WHEN tc.Description = 'Dogs Allowed On-Leash' THEN 1 ELSE 0 END) AS DogsAllowedOnLeash,
			SUM(CASE WHEN tc.Description = 'Dogs Allowed Without Leash' THEN 1 ELSE 0 END) AS DogsAllowedOffLeash,
			SUM(CASE WHEN tc.Description = 'Dogs Not Allowed' THEN 1 ELSE 0 END) AS DogsNotAllowed,
			SUM(CASE WHEN tc.Description = 'May Encounter Pack Animals' THEN 1 ELSE 0 END) AS MayEncounterPackAnimals,
			SUM(CASE WHEN tc.Description = 'May Encounter Mountain Bikes' THEN 1 ELSE 0 END) AS MayEncounterMountainBikes,
			SUM(CASE WHEN tc.Description = 'May Encounter Motorized Vehicles' THEN 1 ELSE 0 END) AS MayEncounterMotorizedVehicles,
			SUM(CASE WHEN tc.Description = 'Permit or Pass Required' THEN 1 ELSE 0 END) AS PermitOrPassRequired
		FROM TrailCharacteristicTrails tct WITH (NOLOCK)
			JOIN TrailCharacteristics tc WITH (NOLOCK) ON tct.TrailCharacteristic_Id = tc.Id
		GROUP BY tct.Trail_Id
	) c ON c.Trail_Id = tra.Id
