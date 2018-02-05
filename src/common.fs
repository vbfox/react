namespace Elmish.React

open Fable.Import.React
open Fable.Helpers.React
open Fable.Core
open Elmish

type [<Pojo>] LazyProps<'model> = {
    key: string
    model:'model
    render:unit->ReactElement
    equal:'model->'model->bool
}

module Components =
    type LazyView<'model>(props) =
        inherit Component<LazyProps<'model>,obj>(props)

        override this.shouldComponentUpdate(nextProps, _nextState) =
            not <| this.props.equal this.props.model nextProps.model

        override this.render () =
            this.props.render ()

[<AutoOpen>]
module Common =
    open Fable.Core.JsInterop
    /// Avoid rendering the view unless the model has changed.
    /// equal: function to compare the previous and the new states
    /// view: function to render the model
    /// state: new state to render
    let lazyViewWith (equal:'model->'model->bool)
                     (view:'model->ReactElement)
                     (state:'model) =
        ofType<Components.LazyView<_>,_,_>
            {
              key = unbox<string> state?key
              render = fun () -> view state
              equal = equal
              model = state }
            []

    /// Avoid rendering the view unless the model has changed.
    /// equal: function to compare the previous and the new states
    /// view: function to render the model using the dispatch
    /// state: new state to render
    /// dispatch: dispatch function
    let lazyView2With (equal:'model->'model->bool)
                      (view:'msg Dispatch->'model->ReactElement)
                      (state:'model)
                      (dispatch:'msg Dispatch) =
        ofType<Components.LazyView<_>,_,_>
            {
              key = unbox<string> state?key
              render = fun () -> view dispatch state
              equal = equal
              model = state }
            []

    [<Emit("$0 || $1")>]
    let private jsOr a b = jsNative

    /// Avoid rendering the view unless the model has changed.
    /// equal: function to compare the previous and the new model (a tuple of two states)
    /// view: function to render the model using the dispatch
    /// state1: new state to render
    /// state2: new state to render
    /// dispatch: dispatch function
    let lazyView3With (equal:_->_->bool) (view:_->_->_->ReactElement) state1 state2 (dispatch:'msg Dispatch) =
        ofType<Components.LazyView<_>,_,_>
            {
              key = unbox<string> (jsOr (state1?key) (state2?key))
              render = fun () -> view dispatch state1 state2
              equal = equal
              model = (state1,state2) }
            []

    /// Avoid rendering the view unless the model has changed.
    /// view: function of model to render the view
    let lazyView (view:'model->ReactElement) =
        lazyViewWith (=) view

    /// Avoid rendering the view unless the model has changed.
    /// view: function of two arguments to render the model using the dispatch
    let lazyView2 (view:'msg Dispatch->'model->ReactElement) =
        lazyView2With (=) view

    /// Avoid rendering the view unless the model has changed.
    /// view: function of three arguments to render the model using the dispatch
    let lazyView3 (view:_->_->_->ReactElement) =
        lazyView3With (=) view


