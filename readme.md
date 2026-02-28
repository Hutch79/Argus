# Argus

A multi Eyed uptime monitoring solution.


---

# What should Argus be

Argus should be a simple to use uptime monitoring solution which allows for multi location monitoring. This results in two distinct use cases.

## Use case 1 - Network restricted services

Imagine a company network or a HomeLab with Services not published to the Internet.

With an external monitoring system, you'd need to set up either a VPN or Socks/http proxy to allow monitoring requests. Both of these solutions need port forwarding, which isn't always an option (e.g. CG Nat or company policies) and open up an entryway for potential attacks if credentials get breached.

An internal only monitoring would be possible, but then you'd be switching between multiple monitoring dashboards to get a full system overview.

## Use case 2 - Geo monitoring

You don't trust your one server location and want to monitor from multiple locations.\nOr you want to measure latency from multiple regions.

You can use existing monitoring SaaS solutions, but their getting expensive fast.


---

Installation and Setup of Argus should be as simple as possible, despite consisting of three to four different containers. The UI will probably be heavily inspired by [UptimeKuma](https://uptimekuma.org/) which is known for its simple way to set up a new monitoring resource.

# System Overview

Argus consists of two main parts which may get more in the future.\nEach part has its dedicated job which can not be done by another part.


![](assets/system-overview.svg " =601x559")

## Argus

Argus is the control plane of the whole application. Here you're creating and managing your Eyes as well as your resources to monitor.

## Eye

An Eye is the endpoint which does the monitoring.

It receives a list of resources from Argus.

## Postgres

Argus does need to store the data received by its eyes, so we need a persistent storage.\nI currently tend to use Postgres since I don't know any better solution.

## Other components
The System Diagram mentions Panoptes and Hera. Those are services planed for the future, but are highly speculative how, when and if they will be implemented.