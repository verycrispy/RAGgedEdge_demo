# 🧠 RAGged Edge Demo

This project showcases a **Retrieval-Augmented Generation (RAG)** implementation using:

- ✅ **Blazor Server** for interactive UI  
- 🤖 **LM Studio** to run a local LLM (e.g., LLaMA)  
- 🗃️ **SQL Server 2022** with vector search via a stored procedure  
- ⚡ **Streaming responses** using `text/event-stream`  
- 🔍 Optional: Integration with **Entity Framework Core** or **Dapper**  

---

## 🚀 What’s in the Demo

- Ask natural language questions  
- Search your own SQL-based knowledge base using vector similarity  
- Receive LLM-generated answers, with step-by-step reasoning (if enabled)  
- Switch between local LLMs in LM Studio via the configuration page  

---

### 1. Requirements
.NET 8 SDK
SQL Server
LM Studio with a supported local LLM

### 2. Setup
In the folder "SqlRagProvider/Setup" you can find a SQL script to setup your database

### 📃 License
MIT
