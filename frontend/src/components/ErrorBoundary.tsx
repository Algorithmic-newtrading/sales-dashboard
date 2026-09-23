import { Component } from "react";
import type { ReactNode } from "react";

interface Props {
  children: ReactNode;
  fallbackTitle?: string;
}

interface State {
  hasError: boolean;
  errorMessage?: string;
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false };

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, errorMessage: error.message };
  }

  componentDidCatch(error: Error) {
    console.error("[ErrorBoundary] caught:", error);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="rounded-2xl border border-rose-500/30 bg-rose-500/5 p-6 text-center">
          <div className="text-rose-400 font-medium mb-1">
            {this.props.fallbackTitle ?? "Не удалось загрузить блок"}
          </div>
          <div className="text-xs text-slate-500 mb-3">
            {this.state.errorMessage ?? "Неизвестная ошибка"}
          </div>
          <button
            onClick={() => this.setState({ hasError: false, errorMessage: undefined })}
            className="px-3 py-1 text-sm rounded-lg border border-[#232a44] text-slate-300 hover:border-rose-500"
          >
            Попробовать снова
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}