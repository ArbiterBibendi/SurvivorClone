shader_type canvas_item;

uniform vec4 flash_color : hint_color = vec4(0.0, 0.0, 0.0, 0.0);
void fragment() {
	COLOR = texture(TEXTURE, UV) + flash_color;
}