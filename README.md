# Software Sorcerers Senior Capstone
![Team Logo](https://github.com/aldenroy/Software_Sorcerers_Capstone/blob/main/Team_Info/team_logo.png?raw=true) \
**Software Sorcerers:** *Turning logic into legend*

## The Team
- Maintainer: [Alden Roy](https://github.com/aldenroy) - [Resume](https://github.com/aldenroy/Software_Sorcerers_Capstone/blob/main/Team_Info/Resumes/alden_resume_1_10.pdf)
- Developer: [Kaitlyn Woodard](https://github.com/Kait12woodard) - [Resume](https://github.com/aldenroy/Software_Sorcerers_Capstone/blob/main/Team_Info/Resumes/CS461KaitlynResume.pdf)
- Developer: [Kira Morgan](https://github.com/kiraryder) - [Resume](https://github.com/aldenroy/Software_Sorcerers_Capstone/blob/main/Team_Info/Resumes/Kira_Resume.pdf)
- Developer: [Brady Parksion](https://github.com/Scriblezz) - [Resume](https://github.com/aldenroy/Software_Sorcerers_Capstone/blob/main/Team_Info/Resumes/Brady_s_Resume.pdf)

## Team Schedule
| Day | Time | Notes |
| --- | ---- | ----- |
| Monday | 4:30pm - 5:30pm | Meeting with Chris |
| Tuesday | 4:00pm - 6:00pm | Team Meeting |
| Wednesday | | |
| Thursday | 4:00pm - 5:00pm | Team Meeting |
| Friday | 4:00pm - 5:00pm | Team Meeting |
| Saturday | 1:00pm - 2:00pm | Weekend Check-in |
| Sunday | 12:00pm | Pull Requests Due |

## About Us

We are a group of seniors at Western Oregon University ready to show off all the skills and knowledge we have gained through out our time here. We are Computer Science Majors with a focus in Software Engineering. Our mission is to create an amazing Capstone project that we all will be proud to show off to our potential new employers. This project will be built using the Agile and Scrum methodologies and utilizing tools like GitHub, VS code, and Azure.

This project aims to teach and give us experience with how to design, plan, organize, and synthesize a significant group software development project. We aim to apply all aspects of contemporary software engineering activities, including planning, requirements elicitation and analysis, software modeling and design, construction, testing, documentation, and deployment. We will participating in stand-up meetings, weekly reviews, project talks, posters and final project presentations.

### Vision Statement

For users with multiple streaming services and a need for enhanced ADA accessibility, Movie Made Easy is an intuitive information system that consolidates movie searches into a single platform. Our solution simplifies content discovery, ensuring users can effortlessly locate the films they want across all their subscribed services.

## Troubleshooting Example

**What broke:** Movie searches intermittently returned empty results and the UI spinner never completed when the upstream movie catalog API timed out.

**How we detected it:** Application logs now emit structured entries such as `ERROR 2026-01-15T20:04:11Z: SearchMovies external provider failure | requestId=0HMRHO1... | query='Inception'`, which surfaced in Application Insights while the UI was still waiting.

**Debugging steps:**
1. Reproduced the timeout by issuing the same search term and confirmed the HTTP 503 response with the correlated `requestId`.
2. Traced the request through the new INFO logs to verify the call never reached the filtering phase, isolating the issue to the external `_movieService` call.
3. Captured diagnostics from the downstream provider showing repeated network timeouts during peak load.

**What was fixed:** Added explicit logging, validated inputs, and wrapped the `_movieService.SearchMoviesAsync` call with graceful error handling so the controller now returns a descriptive 503 response with the `requestId` instead of hanging the UI.

**How it was verified:** Replayed the failing search, observed the user-friendly error payload in the browser network tab, and confirmed the logs show matching INFO start/finish events followed by the controlled ERROR entry, proving the workflow now fails loudly but safely.