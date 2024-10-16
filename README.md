# RabbitMQ Performance Benchmarking

This repository contains benchmarks designed to evaluate the performance of publishing and consuming messages using RabbitMQ in .NET applications. The tests are implemented using BenchmarkDotNet and focus on different methods and batch sizes for message consumption and publishing.

## Overview

The benchmarks are divided into two main categories:

1. **Publishing Messages Benchmark**: Measures the performance of sending messages to RabbitMQ.
2. **Consuming Messages Benchmarks**: Assesses the performance of retrieving messages from RabbitMQ using various methods and batch sizes.

## Benchmarks

### Publishing Messages Benchmark

**File**: `RabbitMqPublishMessagesPerformanceBenchmark.cs`

This benchmark measures the time it takes to publish a large number of messages to a RabbitMQ queue.

- **Objective**: Evaluate the throughput and efficiency of message publishing to RabbitMQ.
- **Implementation**:
  - Establishes a connection and channel with RabbitMQ.
  - Serializes a `SignalEvent` object into JSON format.
  - Publishes a specified number of messages (`TotalMessages`) to a designated queue.
  - Measures the total time taken to publish all messages.

### Consuming Messages Benchmarks

**File**: `RabbitMqGetMessagesPerformanceBenchmark.cs`

These benchmarks measure the performance of consuming messages from RabbitMQ using two different methods: `BasicConsume` and `BasicGet`, with varying batch sizes.

#### Benchmarks:

1. **BenchmarkBasicConsume1000**
   - Consumes messages using `BasicConsume` in batches of 1000.
2. **BenchmarkBasicConsume100**
   - Consumes messages using `BasicConsume` in batches of 100.
3. **BenchmarkBasicGet1000**
   - Consumes messages using `BasicGet` in batches of 1000.
4. **BenchmarkBasicGet100**
   - Consumes messages using `BasicGet` in batches of 100.

- **Objective**: Compare the performance and efficiency of `BasicConsume` and `BasicGet` methods with different batch sizes.
- **Implementation**:
  - **BasicConsume**:
    - Sets up a consumer using `EventingBasicConsumer`.
    - Retrieves messages in batches until the batch size is met or a timeout occurs.
  - **BasicGet**:
    - Synchronously fetches messages in a loop until the batch size is reached.
  - Measures the time taken to consume all messages.

## How to Run the Benchmarks

1. **Set Up RabbitMQ**: Ensure RabbitMQ is running and accessible.
2. **Configure Test Parameters**:
   - Update `TestQueueConfiguration` with appropriate values (`HostName`, `QueueName`, `TotalMessages`, etc.).
3. **Run the Benchmarks**:
   - Use BenchmarkDotNet to execute the benchmarks.
   - Analyze the results to evaluate performance.

## Purpose

The goal of these benchmarks is to provide insights into the performance characteristics of different message publishing and consuming methods in RabbitMQ. Understanding the efficiency of `BasicConsume` and `BasicGet` methods, as well as the impact of batch sizes, will help developers optimize their messaging systems for better performance.
