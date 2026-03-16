using CivicPulse.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var db = serviceProvider.GetRequiredService<AppDbContext>();

        string[] roles = ["Citizen", "Organizer", "Admin"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin user
        var adminEmail = "admin@civicpulse.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Organizer user for seed events
        var orgEmail = "organizer@civicpulse.com";
        var organizer = await userManager.FindByEmailAsync(orgEmail);
        if (organizer == null)
        {
            organizer = new AppUser
            {
                UserName = orgEmail,
                Email = orgEmail,
                DisplayName = "City of Cheney",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(organizer, "Organizer123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(organizer, "Organizer");
        }

        // Seed events if none exist
        if (!await db.Events.AnyAsync())
        {
            var events = new List<Event>
            {
                new()
                {
                    Title = "City Council Meeting: 2026 Budget Review",
                    Description = "The Cheney City Council will review the proposed 2026 municipal budget, including allocations for road repairs, park maintenance, and public safety. Public comment period included.",
                    EventDate = DateTime.UtcNow.AddDays(3),
                    Time = "6:00 PM - 8:00 PM",
                    Location = "Cheney City Hall, 609 2nd St",
                    Category = EventCategory.Education,
                    AiSummary = "The city council is going over next year's budget to decide how to spend tax dollars on things like fixing roads, maintaining parks, and funding police and fire services.",
                    WhyItMatters = "This directly affects how much you pay in local taxes and the quality of roads, parks, and emergency services in your neighborhood.",
                    OrganizerId = organizer!.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Affordable Housing Town Hall",
                    Description = "Join the Spokane Housing Authority for a discussion on expanding affordable housing options. Topics include new zoning proposals, rent stabilization measures, and first-time homebuyer assistance programs.",
                    EventDate = DateTime.UtcNow.AddDays(5),
                    Time = "5:30 PM - 7:30 PM",
                    Location = "Spokane Public Library, 906 W Main Ave",
                    Category = EventCategory.Housing,
                    AiSummary = "Local housing officials are hosting an open discussion about making housing more affordable, including changing zoning rules and helping first-time buyers.",
                    WhyItMatters = "With rent prices rising, these proposals could directly affect what you pay for housing or whether you can afford to buy your first home.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Community Policing Forum",
                    Description = "The Cheney Police Department invites residents to discuss community policing strategies, share concerns about neighborhood safety, and learn about new programs for crime prevention and mental health crisis response.",
                    EventDate = DateTime.UtcNow.AddDays(7),
                    Time = "7:00 PM - 9:00 PM",
                    Location = "Cheney Community Center, 610 G St",
                    Category = EventCategory.PublicSafety,
                    AiSummary = "Local police want to hear your safety concerns and share new programs for preventing crime and responding to mental health emergencies.",
                    WhyItMatters = "This is your chance to directly influence how policing works in your community and learn about resources available during emergencies.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Spokane River Cleanup Day",
                    Description = "Volunteer to help clean up the Spokane River and surrounding trails. Supplies provided. Meet at Riverside State Park. All ages welcome. Community service hours available for students.",
                    EventDate = DateTime.UtcNow.AddDays(10),
                    Time = "9:00 AM - 1:00 PM",
                    Location = "Riverside State Park, Spokane",
                    Category = EventCategory.Environment,
                    AiSummary = "A volunteer event to clean up trash and debris along the Spokane River. Everything you need is provided, and students can earn community service hours.",
                    WhyItMatters = "The Spokane River is a major source of drinking water and recreation — keeping it clean protects both public health and the outdoor spaces we enjoy.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "School Board Meeting: Curriculum Updates",
                    Description = "The Cheney School District Board will discuss proposed updates to the K-12 curriculum, including new STEM programs, mental health education, and changes to graduation requirements.",
                    EventDate = DateTime.UtcNow.AddDays(4),
                    Time = "6:30 PM - 8:30 PM",
                    Location = "Cheney High School Auditorium",
                    Category = EventCategory.Education,
                    AiSummary = "The school board is considering changes to what students learn, including more science and tech classes and mental health education.",
                    WhyItMatters = "These changes shape what future graduates know and can do, affecting college readiness and job prospects for students in the district.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Renters' Rights Workshop",
                    Description = "Free workshop covering Washington state tenant rights, how to handle disputes with landlords, security deposit laws, and resources for renters facing eviction.",
                    EventDate = DateTime.UtcNow.AddDays(6),
                    Time = "2:00 PM - 4:00 PM",
                    Location = "EWU PUB, Room 263",
                    Category = EventCategory.Housing,
                    AiSummary = "A free class teaching renters about their legal rights in Washington, including how to deal with landlord disputes and what to do if you're facing eviction.",
                    WhyItMatters = "Most college students rent, and knowing your rights can save you from losing your deposit or being unfairly evicted.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Fire Safety and Emergency Preparedness Fair",
                    Description = "Learn about fire safety, earthquake preparedness, and emergency evacuation plans. Free smoke detectors and first aid kits for attendees. Demonstrations by local fire department.",
                    EventDate = DateTime.UtcNow.AddDays(12),
                    Time = "10:00 AM - 2:00 PM",
                    Location = "Cheney Fire Station, 420 1st St",
                    Category = EventCategory.PublicSafety,
                    AiSummary = "Get free smoke detectors and first aid kits while learning how to prepare for fires, earthquakes, and other emergencies from local firefighters.",
                    WhyItMatters = "Being prepared for emergencies can save your life. Plus, free safety equipment you probably need but haven't gotten around to buying.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Tree Planting Initiative: Cheney Parks",
                    Description = "Help plant 200 native trees in Cheney parks as part of the city's urban canopy restoration project. No experience needed. Tools and refreshments provided.",
                    EventDate = DateTime.UtcNow.AddDays(14),
                    Time = "8:00 AM - 12:00 PM",
                    Location = "Sutton Park, Cheney",
                    Category = EventCategory.Environment,
                    AiSummary = "The city needs volunteers to plant 200 trees in local parks. No gardening experience required — just show up and they'll teach you what to do.",
                    WhyItMatters = "More trees mean cleaner air, cooler summers, and nicer parks. This is a hands-on way to improve the place where you live.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "EWU Student Government Open Forum",
                    Description = "ASEWU student government hosts an open forum to discuss campus issues including tuition, parking, dining options, and student mental health resources. All EWU students welcome.",
                    EventDate = DateTime.UtcNow.AddDays(2),
                    Time = "12:00 PM - 1:30 PM",
                    Location = "EWU PUB, MPR",
                    Category = EventCategory.Education,
                    AiSummary = "Your student government wants to hear what matters to you — tuition costs, parking problems, food options, and mental health support are all on the table.",
                    WhyItMatters = "Student government controls a significant budget and advocates to university administration on your behalf. This is where your voice actually gets heard.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Zoning Change Public Hearing: Downtown Cheney",
                    Description = "Public hearing on proposed zoning changes to allow mixed-use development in downtown Cheney. Could bring new apartments, shops, and restaurants to the area.",
                    EventDate = DateTime.UtcNow.AddDays(8),
                    Time = "7:00 PM - 9:00 PM",
                    Location = "Cheney City Hall, 609 2nd St",
                    Category = EventCategory.Housing,
                    AiSummary = "The city wants to change zoning rules downtown so developers can build mixed-use buildings with apartments above shops and restaurants.",
                    WhyItMatters = "More housing downtown could lower rents and add walkable restaurants and stores — or it could change the character of the neighborhood. Your input matters.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Neighborhood Watch Kickoff Meeting",
                    Description = "Start a Neighborhood Watch program in your area. Learn how to organize neighbors, report suspicious activity, and work with local law enforcement to keep your street safe.",
                    EventDate = DateTime.UtcNow.AddDays(9),
                    Time = "6:00 PM - 7:30 PM",
                    Location = "Wren Pierson Community Center, Cheney",
                    Category = EventCategory.PublicSafety,
                    AiSummary = "Learn how to set up a Neighborhood Watch where you live — basically organizing your neighbors to look out for each other and communicate with police.",
                    WhyItMatters = "Neighborhoods with active watch programs see less crime. It's a simple way to make where you live safer without waiting for someone else to do it.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Recycling and Composting Workshop",
                    Description = "Learn what can and can't be recycled in Spokane County, how to start composting at home or in your apartment, and how to reduce waste. Free composting bins for first 50 attendees.",
                    EventDate = DateTime.UtcNow.AddDays(11),
                    Time = "1:00 PM - 3:00 PM",
                    Location = "Spokane County Fairgrounds",
                    Category = EventCategory.Environment,
                    AiSummary = "A practical workshop on recycling correctly and starting composting, even in an apartment. The first 50 people get a free compost bin.",
                    WhyItMatters = "Most people recycle wrong, which means it ends up in landfills anyway. Learning to do it right actually makes a difference for the environment.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Financial Literacy Night for Young Adults",
                    Description = "Free workshop on budgeting, student loans, credit scores, and investing basics. Presented by local financial advisors. Pizza provided. Open to ages 18-30.",
                    EventDate = DateTime.UtcNow.AddDays(6),
                    Time = "6:00 PM - 8:00 PM",
                    Location = "EWU JFK Library, Room 200",
                    Category = EventCategory.Education,
                    AiSummary = "A free pizza-and-learn night where financial advisors break down budgeting, student loans, credit scores, and how to start investing — all geared toward young adults.",
                    WhyItMatters = "Most schools don't teach personal finance. Understanding loans and credit now can save you thousands of dollars over your lifetime.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Habitat for Humanity Build Day",
                    Description = "Help build affordable homes for families in need. No construction experience required. Lunch provided. Must be 18+ and wear closed-toe shoes.",
                    EventDate = DateTime.UtcNow.AddDays(15),
                    Time = "8:00 AM - 4:00 PM",
                    Location = "West Plains, Cheney",
                    Category = EventCategory.Housing,
                    AiSummary = "Spend a day helping build a house for a family that needs affordable housing. No experience needed — they teach you everything on site.",
                    WhyItMatters = "Affordable housing is in crisis locally. This is a tangible way to help a real family while learning construction skills.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Crosswalk Safety Initiative Meeting",
                    Description = "Discussion about pedestrian safety improvements near EWU campus and downtown Cheney. Topics include new crosswalk signals, speed reduction zones, and lighting upgrades.",
                    EventDate = DateTime.UtcNow.AddDays(5),
                    Time = "5:00 PM - 6:30 PM",
                    Location = "Cheney City Hall, 609 2nd St",
                    Category = EventCategory.PublicSafety,
                    AiSummary = "The city is planning safety upgrades for pedestrians near campus — better crosswalks, slower speed zones, and brighter street lights.",
                    WhyItMatters = "If you walk or bike around campus, these improvements directly affect your safety. Several students have had close calls at current crossings.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Solar Energy Information Session",
                    Description = "Learn about residential solar panel programs, tax incentives, and community solar options available in Spokane County. Presented by Spokane Clean Energy Alliance.",
                    EventDate = DateTime.UtcNow.AddDays(13),
                    Time = "6:30 PM - 8:00 PM",
                    Location = "Spokane Public Library, 906 W Main Ave",
                    Category = EventCategory.Environment,
                    AiSummary = "Find out how solar panels work for homes, what tax breaks are available, and whether community solar programs make sense if you rent.",
                    WhyItMatters = "Energy costs keep rising. Solar programs and tax incentives can significantly cut your electricity bill, and community solar works even for renters.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Voter Registration Drive",
                    Description = "Register to vote or update your registration. Volunteers will help you understand what's on the upcoming ballot. Nonpartisan event. Bring your ID.",
                    EventDate = DateTime.UtcNow.AddDays(1),
                    Time = "10:00 AM - 4:00 PM",
                    Location = "EWU Campus Mall",
                    Category = EventCategory.Education,
                    AiSummary = "A nonpartisan event where you can register to vote, update your address, and get a plain-language breakdown of what's actually on the ballot.",
                    WhyItMatters = "Local elections often decided by tiny margins directly affect your rent, tuition, and daily life more than national ones. But you have to be registered to vote.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Landlord-Tenant Mediation Clinic",
                    Description = "Free mediation services for tenants and landlords to resolve disputes without going to court. Topics include lease violations, maintenance issues, and noise complaints.",
                    EventDate = DateTime.UtcNow.AddDays(9),
                    Time = "3:00 PM - 6:00 PM",
                    Location = "Spokane County Courthouse, Room 101",
                    Category = EventCategory.Housing,
                    AiSummary = "Free help resolving problems with your landlord — things like repairs not getting done, lease disagreements, or noise issues — without having to go to court.",
                    WhyItMatters = "Legal disputes are expensive and stressful. Free mediation can solve your housing problems faster and without a lawyer.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Active Shooter Preparedness Training",
                    Description = "Free training on how to respond during an active threat situation. Covers Run-Hide-Fight methodology, situational awareness, and emergency communication. Open to all community members.",
                    EventDate = DateTime.UtcNow.AddDays(16),
                    Time = "2:00 PM - 4:00 PM",
                    Location = "EWU PUB, Room 101",
                    Category = EventCategory.PublicSafety,
                    AiSummary = "A training session teaching the Run-Hide-Fight method and how to stay aware of your surroundings in case of an active threat on campus or in public.",
                    WhyItMatters = "Knowing what to do in an emergency saves lives. This training gives you practical skills you hope to never use but should definitely have.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                },
                new()
                {
                    Title = "Air Quality Town Hall: Wildfire Smoke Season",
                    Description = "Discussion on air quality monitoring, health impacts of wildfire smoke, and what the city is doing to prepare for smoke season. Learn about free N95 mask distribution and clean air shelters.",
                    EventDate = DateTime.UtcNow.AddDays(18),
                    Time = "6:00 PM - 7:30 PM",
                    Location = "Cheney Community Center, 610 G St",
                    Category = EventCategory.Environment,
                    AiSummary = "The city is preparing for wildfire smoke season and wants to tell you about free masks, clean air shelters, and how to protect yourself when the air gets bad.",
                    WhyItMatters = "Spokane's smoke seasons are getting worse every year. Knowing where to get free N95 masks and clean air can protect your health.",
                    OrganizerId = organizer.Id,
                    IsApproved = true
                }
            };

            db.Events.AddRange(events);
            await db.SaveChangesAsync();
        }
    }
}
