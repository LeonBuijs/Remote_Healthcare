#version 150

in vec3 a_position;
in vec4 a_color;

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;

out vec4 color;

void main()
{
	color = a_color;
	gl_Position = projectionMatrix * modelViewMatrix * vec4(a_position,1.0);
}