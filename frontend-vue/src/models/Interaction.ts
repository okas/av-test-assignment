export declare interface IInteractionAdd {
  description: string;
  deadline: Date;
}

export declare interface IInteractionEdit extends IInteractionAdd {
  id: string;
  isOpen: boolean;
}

export declare interface IInteractionVm extends IInteractionEdit {
  created: Date;
}
