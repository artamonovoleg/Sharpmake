﻿// Copyright (c) Ubisoft. All Rights Reserved.
// Licensed under the Apache 2.0 License. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sharpmake;

namespace HelloClangCl
{
    [Fragment, Flags]
    public enum Optimization
    {
        Debug = 1 << 0,
        Release = 1 << 1
    }

    [Fragment, Flags]
    public enum Compiler
    {
        MSVC = 1 << 0,
        ClangCl = 1 << 1
    }

    [DebuggerDisplay("\"{Platform}_{DevEnv}\" {Name}")]
    public class CommonTarget : Sharpmake.ITarget
    {
        public Platform Platform;
        public Compiler Compiler;
        public DevEnv DevEnv;
        public Optimization Optimization;
        public Blob Blob;
        public BuildSystem BuildSystem;

        public CommonTarget() { }

        public CommonTarget(
            Platform platform,
            Compiler compiler,
            DevEnv devEnv,
            Optimization optimization,
            Blob blob,
            BuildSystem buildSystem
        )
        {
            Platform = platform;
            Compiler = compiler;
            DevEnv = devEnv;
            Optimization = optimization;
            Blob = blob;
            BuildSystem = buildSystem;
        }

        public override string Name
        {
            get
            {
                var nameParts = new List<string>
                {
                    Compiler.ToString(),
                    Optimization.ToString()
                };
                return string.Join(" ", nameParts);
            }
        }

        public string SolutionPlatformName
        {
            get
            {
                var nameParts = new List<string>();

                nameParts.Add(BuildSystem.ToString());

                if (BuildSystem == BuildSystem.FastBuild && Blob == Blob.NoBlob)
                    nameParts.Add(Blob.ToString());

                return string.Join("_", nameParts);
            }
        }

        /// <summary>
        /// returns a string usable as a directory name, to use for instance for the intermediate path
        /// </summary>
        public string DirectoryName
        {
            get
            {
                var dirNameParts = new List<string>();

                dirNameParts.Add(Platform.ToString());
                dirNameParts.Add(Compiler.ToString());
                dirNameParts.Add(Optimization.ToString());
                dirNameParts.Add(BuildSystem.ToString());

                return string.Join("_", dirNameParts);
            }
        }

        public override Sharpmake.Optimization GetOptimization()
        {
            switch (Optimization)
            {
                case Optimization.Debug:
                    return Sharpmake.Optimization.Debug;
                case Optimization.Release:
                    return Sharpmake.Optimization.Release;
                default:
                    throw new NotSupportedException("Optimization value " + Optimization.ToString());
            }
        }

        public override Platform GetPlatform()
        {
            return Platform;
        }

        public static CommonTarget[] GetDefaultTargets()
        {
            var result = new List<CommonTarget>();
            result.AddRange(GetWin64Targets());
            return result.ToArray();
        }

        public static CommonTarget[] GetWin64Targets()
        {
            var defaultTarget = new CommonTarget(
                Platform.win64,
                Compiler.MSVC | Compiler.ClangCl,
                Globals.DevEnvVersion,
                Optimization.Debug | Optimization.Release,
                Blob.NoBlob,
                BuildSystem.MSBuild
            );

            // make a fastbuild version of the target
            var fastBuildTarget = (CommonTarget)defaultTarget.Clone(
                Blob.FastBuildUnitys,
                BuildSystem.FastBuild
            );

            return new[] { defaultTarget, fastBuildTarget };
        }
    }
}
