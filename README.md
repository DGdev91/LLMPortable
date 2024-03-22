# LLMPortable

![icon](icon.png)

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

LLLPortable is a simple demo for demostrating how to interference an LLM inside a Godot project.

It uses [LLamaSharp](https://github.com/SciSharp/LLamaSharp) for interacting with the LLM.
As backend, it currently uses OpenCL for Windows and Linux, Metal for Mac, and CPU for Android

In this first version it uses [TinyLLama](https://huggingface.co/TheBloke/TinyLlama-1.1B-Chat-v1.0-GGUF) as model, by downloading it from Huggingface.
