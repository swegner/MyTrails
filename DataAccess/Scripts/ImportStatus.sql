DECLARE @timeZone VARCHAR(30) = '-07:00'

SELECT TOP 20
	e.Id,
	CONVERT(NVARCHAR(MAX), SWITCHOFFSET(e.StartTime, @timeZone), 100) AS StartTime,
	CONVERT(NVARCHAR(MAX), SWITCHOFFSET(e.LastHeartbeat, @timeZone), 100) AS LastHeartbeat,
	DATEDIFF(MINUTE, e.StartTime, e.CompletedTime) AS RunTimeMinutes,
	e.CompletedTrailsCount - e.StartTrailsCount TrailsAdded,
	e.CompletedTripReportsCount - e.StartTripReportsCount TripReportsAdded,
	e.ErrorsCount,
	e.ErrorString
FROM ImportLogEntries e WITH (NOLOCK)
ORDER BY e.Id DESC

