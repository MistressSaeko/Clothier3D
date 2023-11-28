using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Clothier3D
{
    public class Shader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string VertPath, string FragPath)
        {
            var ShaderSource = File.ReadAllText(VertPath);

            var VertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(VertexShader, ShaderSource);

            CompileShader(VertexShader);

            ShaderSource = File.ReadAllText(FragPath);
            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, ShaderSource);
            CompileShader(FragmentShader);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, FragmentShader);
            GL.AttachShader(Handle, VertexShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var NumberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < NumberOfUniforms; i++)
            {
                var Key = GL.GetActiveUniform(Handle, i, out _, out _);

                var Location = GL.GetUniformLocation(Handle, Key);

                _uniformLocations.Add(Key, Location);
            }
        }

        private static void CompileShader(int Shader)
        {
            GL.CompileShader(Shader);

            GL.GetShader(Shader, ShaderParameter.CompileStatus, out var Code);

            if(Code != (int)All.True)
            {
                var InfoLog = GL.GetShaderInfoLog(Shader);
                throw new Exception($"Error compiling shader!({Shader}).\n\n{InfoLog}");
            }
        }

        private static void LinkProgram(int Program)
        {
            GL.LinkProgram(Program);

            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out var Code);

            if (Code != (int)All.True)
            {
                throw new Exception($"Error linking program!({Program}).");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttributeLocation(string AttrbuteName)
        {
            return GL.GetAttribLocation(Handle, AttrbuteName);
        }

        public void SetInt(string Name, int Data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[Name], Data);
        }

        public void SetFloat(string Name, float Data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[Name], Data);
        }

        public void SetMatrix4(string Name, Matrix4 Data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[Name], true, ref Data);
        }

        public void SetVector3(string Name, Vector3 Data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[Name], Data);
        }
    }
}
