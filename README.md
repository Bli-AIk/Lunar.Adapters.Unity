This branch (`backup`) serves as an archive of the commit structure before it was reorganized.

It preserves the original state of all commits prior to the refactoring. Moving forward, the primary workflow will be through merging Pull Requests (PRs) to avoid direct commits to the `main` branch.

---

此分支 (`backup`) 是对提交结构进行整理前的版本存档。

该分支保留了重构前，所有提交的原始状态。此后，主要工作流将通过合并PR的方式进行，以避免直接提交到 `main` 分支。

---

# Lunar.Adapters.Unity

> **Status**: 🚧 Initial iteration (features and structure subject to frequent changes)

This repository contains the **Unity adapter** for the **Lunar** game core library. It provides all the necessary components to integrate the pure C\# and engine-agnostic **Lunar ECS** framework into the Unity engine.

The core **Lunar** library is based on [Arch-ECS](https://github.com/genaray/Arch), and its license can be found [here](https://www.google.com/search?q=Licenses/Apache-2.0/LICENSE.MD).

This README is temporary.

## License

Lunar.Adapters.Unity is available under a dual-license model:

### Open Source License (LGPL-3.0)

- You may use Lunar.Adapters.Unity in closed-source Unity projects as long as you do not modify the adapter's source code.
- If you modify this adapter (e.g. add/change core modules or integrations), you must release those modifications under the same license (LGPL-3.0).
- Your own game/application code may remain closed-source.

### Commercial License

- If you wish to modify Lunar.Adapters.Unity and keep those modifications closed-source, or include this adapter in a project where LGPL is not acceptable, you can contact me for a commercial license.

-----

> **状态**：🚧 初始迭代中（功能与结构可能频繁变动）

此仓库包含 **Lunar** 游戏核心库的 **Unity 适配器**。它提供了所有必需的组件，用于将纯 C\#、引擎无关的 **Lunar ECS** 框架集成到 Unity 引擎中。

核心的 **Lunar** 库基于 [Arch-ECS](https://github.com/genaray/Arch) 开发。在 [这里](https://www.google.com/search?q=Licenses/Apache-2.0/LICENSE.MD) 查阅 Arch-ECS 的许可证。

此 Readme 是临时的。

## 许可证

Lunar.Adapters.Unity 采用双重许可模式：

### 开源许可证（LGPL-3.0）

- 只要您不修改本适配器的源代码，就可以在闭源的 Unity 项目中使用 Lunar.Adapters.Unity。
- 如果您修改了本适配器（例如添加 / 更改核心模块或集成），您必须在相同的许可证（LGPL-3.0）下发布这些修改。
- 您自己的游戏 / 应用程序代码可以保持闭源。

### 商业许可

如果您希望修改 Lunar.Adapters.Unity 并保持这些修改闭源，或在不接受 LGPL 的项目中包含此适配器，您可以联系我以获取商业许可。