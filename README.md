# Auction Website

## Overview
The Auction Website is a web-based platform that allows users to create, bid on, and manage online auctions. 
It provides a marketplace where users can list items for sale, participate in real-time bidding, and manage their transactions securely.

## Purpose
The goal of this project is to create a user-friendly and reliable auction system where individuals can sell items to the highest bidder. 
The platform ensures fair competition by implementing real-time bid updates, authentication, and financial transactions.

## Key Features
- **User Authentication & Registration**: Users can create an account, log in, and manage their profile.
- **Auction Listings**: Users can post new auctions, including item descriptions, starting bids, and end dates.
- **Bidding System**: Registered users can bid on active auctions, with real-time updates on current bids.
- **Real-Time Updates**: Implemented using SignalR, users see immediate updates when new bids are placed.
- **Wallet Management**: Each user starts with a balance of $1000, which is used for bidding. Funds are deducted automatically when a user wins an auction.
- **Auction Expiration**: Auctions close automatically once the time expires, and the highest bidder wins.
- **Transaction Processing**: Upon auction completion, funds are transferred from the winning bidder to the auction creator.
- **Email Notifications**: Users receive notifications for registration, bid updates, and auction wins.
- **Logging System**: The system logs user actions, transactions, and errors for security and debugging purposes.

## Target Audience
This project is intended for individuals who want to buy and sell items through an auction system. It can be used for general consumer goods, collectibles, or niche markets such as artwork and antiques.

## Technologies Used
- **.NET Core** (for backend development)
- **Entity Framework Core** (for database management)
- **SQL Server** (for data storage)
- **ASP.NET MVC** (for web application structure)
- **SignalR** (for real-time bid updates)
- **jQuery & Bootstrap** (for frontend enhancements)
- **MailJet API** (for email notifications)
- **Logging Framework** (for tracking system activity)

## Future Enhancements
- Implementing **auction categories** to organize listings.
- Adding **automatic bid increments** to streamline the bidding process.
- Introducing **payment gateway integration** for external financial transactions.
- Enhancing **mobile responsiveness** for better user experience on all devices.

## Conclusion
The Auction Website provides a seamless online auction experience with secure bidding, automated transactions, and real-time updates. 
It is designed to be scalable, allowing further enhancements and integrations in the future.