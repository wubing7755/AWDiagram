export const ScriptsInOne = (() => {
    return {
        initialize() {
            console.log('Initializing scripts...');

            // 挂载侧边栏拖拽
            if (typeof initSidebarDrag === 'function') {
                window.initSidebarDrag();
            }

            // 挂载主题切换
            if (typeof initThemeToggle === 'function') {
                window.initThemeToggle();
            }

            // 关闭页面前执行销毁任务
            window.addEventListener('beforeunload', () => this.cleanup());
        },
        cleanup() {
            // 执行主题切换清理
            document.documentElement.removeAttribute('data-theme');
        },
        forcecleanup() {
            /**
             * 强制清除
             */

            this.cleanup();

            localStorage.removeItem('theme');
        }
    };
})();

window.ScriptsInOne = ScriptsInOne;


/* ------------------------ SVG Elements Start ------------------------ */

class SVGDragController {
    /**
     * 构造函数
     * @param {SVGElement} element 
     * @param {Object} dotNetObjRef 
     * @param {number} initialX 
     * @param {number} initialY 
     */
    constructor(element, dotNetObjRef, initialX, initialY) {
        if (!element || !(element instanceof SVGElement)) {
            throw new Error('Invalid SVG element provided');
        }

        this.element = element;
        this.dotNetObjRef = dotNetObjRef;
        this.svg = element.ownerSVGElement;

        // 拖拽状态
        this.state = {
            isSelected: false,
            isDragging: false,
            startOffset: { x: 0, y: 0 },
            position: { x: initialX, y: initialY }
        };

        // 绑定事件处理器，确保this上下文正确
        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.handleMouseMove = this.handleMouseMove.bind(this);
        this.handleMouseUp = this.handleMouseUp.bind(this);
        this.handleDocumentClick = this.handleDocumentClick.bind(this);
    }

    /**
     * 初始化SVG元素拖拽功能
     * @param {SVGElement} element - 要添加拖拽功能的SVG元素
     * @param {Object} dotNetObjRef - Blazor组件引用
     * @param {number} initialX - 初始X坐标
     * @param {number} initialY - 初始Y坐标
     * @returns {Function} 清理函数，用于移除事件监听
     */
    initialize() {
        this.element.style.cursor = 'move';
        this.element.setAttribute('data-draggable', 'true');
        this.element.addEventListener('mousedown', this.handleMouseDown);
        this.svg.addEventListener('mousemove', this.handleMouseMove);
        this.svg.addEventListener('mouseup', this.handleMouseUp);
        document.addEventListener('click', this.handleDocumentClick);
    }

    /**
     * 清理事件监听
     */
    dispose() {
        this.element.removeEventListener('mousedown', this.handleMouseDown);
        this.svg.removeEventListener('mousemove', this.handleMouseMove);
        this.svg.removeEventListener('mouseup', this.handleMouseUp);
        document.removeEventListener('click', this.handleDocumentClick);

        this.element.style.cursor = '';
        this.element.removeAttribute('data-draggable');
    }

    /**
     * 鼠标按下事件处理
     * @param {MouseEvent} event 
     */
    async handleMouseDown(event) {
        // 只响应左键点击
        if (event.button !== 0) return;

        const svgPoint = this.getSVGPoint(event.clientX, event.clientY);

        this.state.isSelected = true;
        this.state.isDragging = true;
        this.state.startOffset = {
            x: svgPoint.x - this.state.position.x,
            y: svgPoint.y - this.state.position.y
        };

        await this.dotNetObjRef.invokeMethodAsync('SelectedElement');

        event.preventDefault();
        event.stopPropagation();
    }

    /**
     * 鼠标移动事件处理
     * @param {MouseEvent} event 
     */
    handleMouseMove(event) {
        if (!this.state.isDragging) return;

        const svgPoint = this.getSVGPoint(event.clientX, event.clientY);

        this.state.position = {
            x: svgPoint.x - this.state.startOffset.x,
            y: svgPoint.y - this.state.startOffset.y
        };

        this.notifyPositionUpdate();

        event.preventDefault();
        event.stopPropagation();
    }

    /**
     * 鼠标释放事件处理
     */
    async handleMouseUp() {
        this.state.isDragging = false;
    }

    async handleDocumentClick(event) {
        // 检查点击是否发生在当前控制器管理的元素或其子元素上
        const isClickOnSelfOrChild = this.element.contains(event.target) ||
            event.target === this.element;

        // 检查点击是否发生在SVG画布内（包括所有子元素）
        const isClickInSVG = this.svg.contains(event.target);

        // 如果点击在SVG画布内，在当前元素/子元素外，且当前元素/子元素是选中状态
        if ((isClickInSVG && !isClickOnSelfOrChild) && this.state.isSelected) {
            this.state.isSelected = false;
            await this.dotNetObjRef.invokeMethodAsync('UnSelectedElement');
        }
    }

    /**
     * 通知.NET位置更新
     */
    async notifyPositionUpdate() {
        try {
            await this.dotNetObjRef.invokeMethodAsync(
                'UpdatePosition',
                this.state.position.x,
                this.state.position.y
            );
        } catch (error) {
            console.error('Failed to notify position update:', error);
            // 可以考虑在这里添加重试逻辑或错误上报
        }
    }

    /**
     * 获取SVG坐标系中的点
     * @param {number} clientX 
     * @param {number} clientY 
     * @returns {SVGPoint}
     */
    getSVGPoint(clientX, clientY) {
        const point = this.svg.createSVGPoint();
        point.x = clientX;
        point.y = clientY;
        return point.matrixTransform(this.svg.getScreenCTM().inverse());
    }
}

const controllerInstances = new WeakMap();

export async function initializeDraggableSVGElement(inputElement, dotNetObjRef, x, y) {
    const controller = new SVGDragController(inputElement, dotNetObjRef, x, y);
    controllerInstances.set(inputElement, controller);
    controller.initialize();
}

export async function cleanUpDraggableSVGElement(inputElement) {
    if (!inputElement || !controllerInstances.has(inputElement)) {
        console.warn('No drag controller found for element:', inputElement);
        return;
    }

    const controller = controllerInstances.get(inputElement);
    controller.dispose();
    controllerInstances.delete(inputElement);
}

window.initializeDraggableSVGElement = initializeDraggableSVGElement;
window.cleanUpDraggableSVGElement = cleanUpDraggableSVGElement;
/* ------------------------ SVG Elements End ------------------------ */
