# 🧠 RAGged Edge Demo

This project showcases a **Retrieval-Augmented Generation (RAG)** implementation using:

- ✅ **Blazor Server** for interactive UI  
- 🤖 **LM Studio** to run a local LLM (e.g., LLaMA)  
- 🗃️ **SQL Server 2022** with vector search via a stored procedure  
- ⚡ **Streaming responses** using `text/event-stream`  

---

## 🚀 What’s in the Demo

- Vectorize documents in the database via the configuration page
- Ask natural language questions  
- Search your own SQL-based knowledge base using vector similarity  
- Receive LLM-generated answers
- Switch between local LLMs in LM Studio via the configuration page  

---
### Setup
In the folder "SqlRagProvider/Setup" you can find a SQL script to setup your database

### Requirements
.NET 8 SDK
SQL Server
LM Studio with a supported local LLM

### License
MIT
