interface NinjaJS {
    Stage: (elem: any, canvasWidth: any, canvasHeight: any, actualWidth: any, actualHeight: any) => void;
    Shape: (drawFunc: any) => void;
}
interface Shape {
	draggable;
	drawFunc();
	redraw();
	getCanvas();
	setPosition(x: number, y: number);
	bind(eventName: string, func: Function);
	trigger(eventName: string, e);
}
interface Stage {
	elem: any;
	canvasWidth: any;
	canvasHeight: any;
	actualWidth: any;
	actualHeight: any;

	new (canvas: any, canvasWidth: number, canvasHeight: number, actualWidth: number, actualHeight: number): Stage;
}